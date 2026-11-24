using System;
using System.Web;
using System.Web.UI;

namespace eStoreJA
{
    /// <summary>
    /// The old one-product order form, replaced by Shop → Product → Cart → Checkout.
    /// Old links and bookmarks are sent to the matching new page:
    /// ?productId=6 → Product?id=6; ?category= / ?q= → Shop with the same values; anything else → Shop.
    /// </summary>
    public partial class PlaceOrder : Page
    {
        /// <summary>Redirects before anything is rendered.</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            int productId;
            if (int.TryParse(Request.QueryString["productId"], out productId) && productId > 0)
            {
                Response.Redirect("~/Product?id=" + productId, true);
                return;
            }

            // Pass category / q through unchanged (re-encoded, so nothing else in the URL is carried over)
            string target = "~/Shop";
            string category = Request.QueryString["category"];
            string search = Request.QueryString["q"];
            string query = "";
            if (!string.IsNullOrWhiteSpace(category)) query += "&category=" + HttpUtility.UrlEncode(category);
            if (!string.IsNullOrWhiteSpace(search)) query += "&q=" + HttpUtility.UrlEncode(search);
            if (query.Length > 0) target += "?" + query.Substring(1);

            Response.Redirect(target, true);
        }
    }
}
