using System;
using System.Collections.Generic;
using System.Data.Entity;   // .Include()
using System.Linq;
using System.Web;           // HttpUtility
using System.Web.UI;
using eStoreJA.Models;
using eStoreJA.Services;

namespace eStoreJA
{
    public partial class _Default : Page
    {
        /// <summary>Department names for the storefront dropdown beside each hero slide's button.</summary>
        protected List<string> ShopCategoryNames { get; private set; } = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // A featured card's "Add to cart" button posted its product id
            int addId;
            if (IsPostBack && int.TryParse(Request.Form["addProduct"], out addId))
            {
                using (var db = new ApplicationDbContext())
                    CartService.SetFlash(Context, new CartService(Context, db).Add(addId, 1));
                Response.Redirect("~/", true);   // Post/Redirect/Get: refreshing won't add it again; the toast shows on the next view
            }

            if (!IsPostBack)
            {
                using (var db = new ApplicationDbContext())
                {
                    LoadCategories(db);
                    LoadFeaturedProducts(db);
                }
            }
        }

        /// <summary>Fills the department links under the hero from the Categories table.</summary>
        private void LoadCategories(ApplicationDbContext db)
        {
            var categories = db.Categories.OrderBy(c => c.CategoryId).ToList();
            rptCategories.DataSource = categories;
            rptCategories.DataBind();
            ShopCategoryNames = categories.Select(c => c.Name).ToList();
        }

        /// <summary>Loads the featured products (with their category) into the product grid.
        /// Hidden products (IsActive = false) are never shown; out-of-stock ones show "Out of stock".</summary>
        private void LoadFeaturedProducts(ApplicationDbContext db)
        {
            rptFeatured.DataSource = db.Products
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderBy(p => p.ProductId)
                .ToList();
            rptFeatured.DataBind();
        }

        /// <summary>Link to the Shop filtered to one department, e.g. Shop?category=Home%20%26%20Living</summary>
        protected string CategoryUrl(string categoryName)
        {
            return ResolveUrl("~/Shop?category=" + HttpUtility.UrlEncode(categoryName));
        }

        /// <summary>Local "~/" paths are resolved; full links (e.g. S3) are used as they are.</summary>
        protected string ImageSrc(string url)
        {
            // No picture (e.g. removed in Admin): the bag icon shows instead, but data-binding still
            // evaluates this inside the hidden PlaceHolder, so it must not crash on null.
            if (string.IsNullOrEmpty(url)) return "";
            return url.StartsWith("~") ? ResolveUrl(url) : url;
        }
    }
}