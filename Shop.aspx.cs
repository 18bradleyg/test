using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using eStoreJA.Models;
using eStoreJA.Services;

namespace eStoreJA
{
    /// <summary>One product card on the Shop page.</summary>
    public class ShopCard
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; }
    }

    /// <summary>
    /// Shop: every active product, filtered by ?category=, searched by ?q=, sorted by ?sort= and paged by ?page=
    /// (30 per page). Open to guests. "Add to cart" posts the product id and redirects back (Post/Redirect/Get).
    /// </summary>
    public partial class Shop : Page
    {
        /// <summary>Allowed ?sort= values; anything else falls back to "featured".</summary>
        private static readonly string[] Sorts = { "featured", "price-asc", "price-desc", "name" };

        /// <summary>The selected department's exact name, or null for All (unknown names fall back to All).</summary>
        protected string Category { get; private set; }

        /// <summary>The search text (trimmed, at most 100 characters), or "".</summary>
        protected string Search { get; private set; } = "";

        /// <summary>The selected sort (always one of <see cref="Sorts"/>).</summary>
        protected string Sort { get; private set; } = "featured";

        /// <summary>Department names for the pills (the master's cached list).</summary>
        protected List<string> Departments { get; private set; } = new List<string>();

        /// <summary>The products on this page.</summary>
        protected List<ShopCard> Cards { get; private set; } = new List<ShopCard>();

        /// <summary>"11 products across 6 departments", "5 products", "3 products in Electronics".</summary>
        protected string CountText { get; private set; } = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-teal";

            // "Add to cart" on a card posted its product id: add 1, then come back to the same list
            int addId;
            if (IsPostBack && int.TryParse(Request.Form["addProduct"], out addId))
            {
                using (var db = new ApplicationDbContext())
                    CartService.SetFlash(Context, new CartService(Context, db).Add(addId, 1));
                Response.Redirect(ResolveUrl("~/Shop") + Request.Url.Query, true);   // same filters / page; refresh won't add again
            }

            ReadFilters();
            LoadProducts();
        }

        /// <summary>Reads and whitelists category, q and sort from the query string.</summary>
        private void ReadFilters()
        {
            Departments = SiteMaster.GetCategoryNames();

            string category = Request.QueryString["category"];
            Category = Departments.FirstOrDefault(n => string.Equals(n, category, StringComparison.OrdinalIgnoreCase));

            Search = (Request.QueryString["q"] ?? "").Trim();
            if (Search.Length > 100) Search = Search.Substring(0, 100);

            string sort = (Request.QueryString["sort"] ?? "").ToLowerInvariant();
            Sort = Sorts.Contains(sort) ? sort : "featured";
        }

        /// <summary>Counts, sorts and pages the matching products (one count query, one page query).</summary>
        private void LoadProducts()
        {
            using (var db = new ApplicationDbContext())
            {
                var query = db.Products.Where(p => p.IsActive);
                if (Category != null)
                    query = query.Where(p => p.Category.Name == Category);
                if (Search.Length > 0)   // EF turns Contains into a parameterised SQL LIKE
                    query = query.Where(p => p.Name.Contains(Search) || p.Description.Contains(Search));

                int total = query.Count();
                CountText = Plural(total, "product");
                if (Search.Length > 0 && Category != null)
                    CountText += " in " + Category;
                else if (Search.Length == 0 && Category == null)
                    CountText += " across " + Plural(query.Select(p => p.CategoryId).Distinct().Count(), "department");
                if (Search.Length > 0) CountText += " · ";   // separates the count from "Clear search"

                // In-stock first (sold-out items stay listed, at the end), then the chosen sort
                IOrderedQueryable<Product> ordered = query.OrderBy(p => p.StockQuantity > 0 ? 0 : 1);
                switch (Sort)
                {
                    case "price-asc": ordered = ordered.ThenBy(p => p.UnitPrice).ThenBy(p => p.Name); break;
                    case "price-desc": ordered = ordered.ThenByDescending(p => p.UnitPrice).ThenBy(p => p.Name); break;
                    case "name": ordered = ordered.ThenBy(p => p.Name); break;
                    default: ordered = ordered.ThenByDescending(p => p.IsFeatured).ThenBy(p => p.Name); break;
                }
                ordered = ordered.ThenBy(p => p.ProductId);   // stable order across pages

                int page = Paging.Clamp(Paging.ReadPage(Request, "page"), total, Paging.PageSize);
                Cards = ordered.Skip(Paging.Skip(page)).Take(Paging.PageSize)
                    .Select(p => new ShopCard
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        UnitPrice = p.UnitPrice,
                        ImageUrl = p.ImageUrl,
                        StockQuantity = p.StockQuantity,
                        CategoryName = p.Category.Name
                    })
                    .ToList();

                pager.TotalCount = total;
                pager.CurrentPage = page;
            }

            Title = Search.Length > 0 ? "Search: " + Search : (Category != null ? "Shop " + Category : "Shop");
        }

        /// <summary>
        /// Shop link with the given filters (page dropped, default sort left out). Values are URL-encoded;
        /// the result is HTML-encoded where it is written.
        /// </summary>
        protected string ShopUrl(string category, string q, string sort)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(category)) parts.Add("category=" + HttpUtility.UrlEncode(category));
            if (!string.IsNullOrEmpty(q)) parts.Add("q=" + HttpUtility.UrlEncode(q));
            if (!string.IsNullOrEmpty(sort) && sort != "featured") parts.Add("sort=" + HttpUtility.UrlEncode(sort));
            return ResolveUrl("~/Shop") + (parts.Count > 0 ? "?" + string.Join("&", parts) : "");
        }

        /// <summary>aria-current="true" on the selected sort link (a fixed string, safe to write raw).</summary>
        protected string SortCurrent(string sort)
        {
            return sort == Sort ? "aria-current=\"true\"" : "";
        }

        /// <summary>"1 product", "5 products".</summary>
        private static string Plural(int n, string word)
        {
            return n + " " + word + (n == 1 ? "" : "s");
        }
    }
}
