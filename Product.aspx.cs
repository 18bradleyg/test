using System;
using System.Collections.Generic;
using System.Data.Entity;   // Include
using System.Linq;
using System.Web;
using System.Web.UI;
using eStoreJA.Models;
using eStoreJA.Services;

namespace eStoreJA
{
    /// <summary>A small card in "More from {Category}".</summary>
    public class RelatedProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
    }

    /// <summary>
    /// Product page (Product?id=6). Open to guests. Unknown, invalid or hidden ids show "Product not found"
    /// with HTTP 404. "Add to cart" and "Buy now" post the quantity; the server caps it at stock and
    /// redirects (Buy now → Checkout). Named ProductPage so it doesn't clash with the Models.Product entity.
    /// </summary>
    public partial class ProductPage : Page
    {
        /// <summary>The product, or null when not found.</summary>
        protected Models.Product Item { get; private set; }

        /// <summary>Up to 4 other active products from the same department.</summary>
        protected List<RelatedProduct> More { get; private set; } = new List<RelatedProduct>();

        /// <summary>Result of the add just made (shown once after the redirect), or null.</summary>
        protected AddResult Added { get; private set; }

        /// <summary>Name without its last word, e.g. "Bluetooth " (the last word is shown bold italic).</summary>
        protected string NameStart { get; private set; } = "";

        /// <summary>Last word of the name, e.g. "Headphones".</summary>
        protected string NameLast { get; private set; } = "";

        /// <summary>True when the product has no stock (stepper and buttons disabled).</summary>
        protected bool SoldOut { get { return Item == null || Item.StockQuantity <= 0; } }

        /// <summary>Highest quantity the stepper allows: stock, but at most 99.</summary>
        protected int MaxQuantity { get { return Item == null ? 1 : Math.Max(1, Math.Min(Item.StockQuantity, CartService.MaxQuantity)); } }

        /// <summary>Shop filtered to this product's department.</summary>
        protected string CategoryUrl
        {
            get { return ResolveUrl("~/Shop?category=" + HttpUtility.UrlEncode(Item.Category.Name)); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-teal";

            int id;
            using (var db = new ApplicationDbContext())
            {
                if (int.TryParse(Request.QueryString["id"], out id))
                {
                    Item = db.Products.Include(p => p.Category)
                             .FirstOrDefault(p => p.ProductId == id && p.IsActive);
                }

                if (Item == null)
                {
                    Title = "Product not found";
                    Response.StatusCode = 404;
                    Response.TrySkipIisCustomErrors = true;   // keep this page instead of IIS's own 404 page
                    return;
                }

                if (IsPostBack) HandleAdd(db);   // redirects

                Added = CartService.TakeFlash(Context);   // taken here, so the master's toast doesn't show it as well
                LoadRelated(db);
            }

            Title = Item.Name;
            MetaDescription = Item.Description ?? "";

            int space = Item.Name.TrimEnd().LastIndexOf(' ');
            NameStart = space > 0 ? Item.Name.Substring(0, space + 1) : "";
            NameLast = space > 0 ? Item.Name.Substring(space + 1) : Item.Name;
        }

        /// <summary>
        /// Add to cart / Buy now: reads the posted quantity (defaults to 1, kept within 1–99), adds it, then
        /// redirects – to Checkout for Buy now, otherwise back here to show the confirmation bar.
        /// </summary>
        private void HandleAdd(ApplicationDbContext db)
        {
            bool buyNow = Request.Form["buyNow"] != null;
            if (!buyNow && Request.Form["addToCart"] == null) return;

            int qty;
            if (!int.TryParse(Request.Form["qty"], out qty)) qty = 1;
            qty = Math.Max(1, Math.Min(qty, CartService.MaxQuantity));

            var result = new CartService(Context, db).Add(Item.ProductId, qty);
            if (buyNow && result.Success) Response.Redirect("~/Checkout", true);

            CartService.SetFlash(Context, result);
            Response.Redirect("~/Product?id=" + Item.ProductId, true);
        }

        /// <summary>Up to 4 other products from the department, in-stock and featured first.</summary>
        private void LoadRelated(ApplicationDbContext db)
        {
            More = db.Products
                .Where(p => p.IsActive && p.CategoryId == Item.CategoryId && p.ProductId != Item.ProductId)
                .OrderBy(p => p.StockQuantity > 0 ? 0 : 1)
                .ThenByDescending(p => p.IsFeatured)
                .ThenBy(p => p.Name)
                .Take(4)
                .Select(p => new RelatedProduct { ProductId = p.ProductId, Name = p.Name, UnitPrice = p.UnitPrice, ImageUrl = p.ImageUrl })
                .ToList();
        }
    }
}
