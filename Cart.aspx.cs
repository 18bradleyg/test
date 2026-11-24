using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using eStoreJA.Models;
using eStoreJA.Services;

namespace eStoreJA
{
    /// <summary>
    /// Shopping cart. Open to guests (their cart is in Session). Quantity − / + and Remove are POST buttons;
    /// each change is applied by CartService and the page redirects back to itself (Post/Redirect/Get).
    /// </summary>
    public partial class Cart : Page
    {
        private const string StatusKey = "CartStatus";

        /// <summary>The cart lines with current prices.</summary>
        protected IList<CartLine> Lines { get; private set; } = new List<CartLine>();

        /// <summary>What CartService changed by itself (hidden / sold-out products, lowered quantities).</summary>
        protected IList<string> Notices { get; private set; } = new List<string>();

        /// <summary>Result of the last button, read out by screen readers ("Removed X from your cart.").</summary>
        protected string Status { get; private set; } = "";

        /// <summary>Total number of items.</summary>
        protected int ItemCount { get { return Lines.Sum(l => l.Quantity); } }

        /// <summary>"1 item" / "6 items".</summary>
        protected string ItemCountText { get { return ItemCount + (ItemCount == 1 ? " item" : " items"); } }

        /// <summary>Σ current price × quantity.</summary>
        protected decimal Subtotal { get { return Lines.Sum(l => l.LineTotal); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-teal";

            using (var db = new ApplicationDbContext())
            {
                var cart = new CartService(Context, db);

                if (IsPostBack)
                {
                    Session[StatusKey] = ApplyChange(cart, db);
                    Response.Redirect("~/Cart", true);   // a refresh won't repeat the change
                }

                IList<string> notices;
                Lines = cart.GetLines(out notices);
                Notices = notices;
            }

            Status = Session[StatusKey] as string ?? "";
            Session.Remove(StatusKey);
        }

        /// <summary>Applies the posted −, + or Remove button and returns a short status message.</summary>
        private string ApplyChange(CartService cart, ApplicationDbContext db)
        {
            int id;

            if (int.TryParse(Request.Form["remove"], out id))
            {
                string name = db.Products.Where(p => p.ProductId == id).Select(p => p.Name).FirstOrDefault() ?? "The item";
                cart.Remove(id);
                return "Removed " + name + " from your cart.";
            }

            bool up = int.TryParse(Request.Form["inc"], out id);
            if (up || int.TryParse(Request.Form["dec"], out id))
            {
                int before = cart.QuantityOf(id);
                cart.SetQuantity(id, before + (up ? 1 : -1));   // − at 1 removes the line; + is capped at stock
                int after = cart.QuantityOf(id);
                if (up && after == before && before > 0) return "That's all we have in stock.";
                return after == 0 ? "Item removed from your cart." : "Quantity updated to " + after + ".";
            }

            return "";
        }
    }
}
