using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;     // RefreshMode
using System.Data.Entity.Infrastructure;   // IObjectContextAdapter
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;           // User.Identity.GetUserId()
using Shared.Contracts;                    // PaymentRequest, PaymentResponse
using eStoreJA.Models;
using eStoreJA.Services;

namespace eStoreJA
{
    /// <summary>
    /// Checkout for the whole cart (signed-in users only; guests go to Log in and come back).
    /// Same rules as the old Place Order page, generalised to many items: card checked by CreditCard.Api,
    /// order saved as Paid with one OrderItem per line and stock reduced in one SaveChanges, then shipped
    /// (Shipped + tracking number) or left as Awaiting Shipment if Shipping.Api is down.
    /// </summary>
    public partial class Checkout : SecurePage
    {
        private const string ShownTotalKey = "ShownTotal";

        /// <summary>The lines shown in the summary (current prices).</summary>
        protected IList<CartLine> Lines { get; private set; }

        /// <summary>Σ price × quantity of <see cref="Lines"/>.</summary>
        protected decimal Total { get { return Lines == null ? 0m : Lines.Sum(l => l.LineTotal); } }

        /// <summary>True once the order is saved: the confirmation panel replaces the form.</summary>
        protected bool Done { get; private set; }

        // Confirmation panel values
        protected string DoneName { get; private set; }
        protected int DoneOrderId { get; private set; }
        protected string DoneTracking { get; private set; }
        protected decimal DoneTotal { get; private set; }
        protected bool DoneShipped { get; private set; }

        /// <summary>Track package link: straight to this order's tracking number when there is one.</summary>
        protected string TrackUrl
        {
            get
            {
                return string.IsNullOrEmpty(DoneTracking)
                    ? ResolveUrl("~/TrackPackage")
                    : ResolveUrl("~/TrackPackage?tracking=" + HttpUtility.UrlEncode(DoneTracking));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-teal";
            pnlAlert.Visible = false;   // only shown by this request's ShowAlert (Visible is kept in ViewState otherwise)

            if (IsPostBack) return;   // the click handler loads the cart itself (so it sees any changes first)

            using (var db = new ApplicationDbContext())
            {
                IList<string> notices;
                Lines = new CartService(Context, db).GetLines(out notices);
                if (Lines.Count == 0)
                {
                    Response.Redirect("~/Cart", true);
                    return;
                }
                if (notices.Count > 0) ShowNotices(notices);

                PrefillAddress(db);
            }
        }

        /// <summary>
        /// Copies the user's default address (or newest saved one) from the address book into the form.
        /// The delivery name isn't stored, so it stays empty.
        /// </summary>
        private void PrefillAddress(ApplicationDbContext db)
        {
            var last = new AddressBookService(db, User.Identity.GetUserId()).GetDefault();
            if (last == null)
            {
                txtCountry.Text = "Jamaica";
                return;
            }

            txtLine1.Text = last.AddressLine1;
            txtLine2.Text = last.AddressLine2;
            txtCity.Text = last.City;
            txtState.Text = last.State;
            txtCountry.Text = last.Country;
            txtZip.Text = last.ZipCode;
        }

        /// <summary>
        /// Runs after the (async) click handler has finished: loads the summary if the handler didn't,
        /// sets the button text and remembers the total the customer is looking at.
        /// </summary>
        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (!Done)
            {
                if (Lines == null)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        IList<string> notices;
                        Lines = new CartService(Context, db).GetLines(out notices);
                        if (notices.Count > 0) ShowNotices(notices);
                    }
                }

                if (Lines.Count == 0)
                {
                    ShowAlert("Your cart is empty.", " <a href=\"" + HttpUtility.HtmlAttributeEncode(ResolveUrl("~/Shop")) + "\">Find something in the shop</a>.");
                    btnPlaceOrder.Enabled = false;
                }

                btnPlaceOrder.Text = "Place order · " + CartService.Price(Total);
                ViewState[ShownTotalKey] = Total;   // compared on Place order, so we never charge a total the customer didn't see
            }

            pnlCheckout.Visible = !Done;
            pnlDone.Visible = Done;
            base.OnPreRenderComplete(e);
        }

        /// <summary>Server-side check that the card has not expired (same rule as the old Place Order page).</summary>
        protected void ValidateExpiry(object source, ServerValidateEventArgs args)
        {
            DateTime expiry;
            if (DateTime.TryParseExact(args.Value, "MM/yy", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out expiry))
            {
                DateTime lastDayOfMonth = expiry.AddMonths(1).AddDays(-1);
                args.IsValid = lastDayOfMonth >= DateTime.Today;
            }
            else
            {
                args.IsValid = false;   // the regex validator already shows the format message
            }
        }

        /// <summary>
        /// Places the order for every cart line: checks the cart hasn't changed, authorizes the payment,
        /// saves the order as Paid (re-checking stock), clears the cart, then requests shipping.
        /// </summary>
        protected async void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (!IsValid) return;

            string userId = User.Identity.GetUserId();

            using (var db = new ApplicationDbContext())
            {
                var cart = new CartService(Context, db);

                // ----------  1. FRESH CART (prices and stock from the DB, never from the page) ----------
                IList<string> notices;
                var lines = cart.GetLines(out notices);
                Lines = lines;
                if (lines.Count == 0) return;   // OnPreRenderComplete shows "Your cart is empty."
                if (notices.Count > 0)
                {
                    ShowNotices(notices);   // something was removed or capped: let them review before paying
                    return;
                }

                // ----------  2. TOTAL ----------
                decimal total = lines.Sum(l => l.LineTotal);
                object shown = ViewState[ShownTotalKey];
                if (shown is decimal && (decimal)shown != total)
                {
                    ShowAlert("Prices in your cart changed.", " The new total is " + HttpUtility.HtmlEncode(CartService.Price(total)) +
                              ". Please check it and place your order again. Nothing was charged.");
                    return;
                }

                // ----------  3. AUTHORIZE PAYMENT ----------
                PaymentResponse payment;
                try
                {
                    payment = await ApiClient.AuthorizePaymentAsync(new PaymentRequest
                    {
                        FullName = txtFullName.Text.Trim(),
                        CardNumber = txtCardNumber.Text.Trim(),
                        ExpiryDate = txtExpiry.Text.Trim(),
                        Cvv = txtCvv.Text.Trim(),
                        Amount = total
                    });
                }
                catch (Exception)
                {
                    // API is down, timed out, or returned an error
                    ShowAlert(null, "We couldn't reach the payment service right now. Please try again later.");
                    return;
                }

                if (payment == null || !string.Equals(payment.Status, "approved", StringComparison.OrdinalIgnoreCase))
                {
                    ShowAlert("Payment declined:", " " + HttpUtility.HtmlEncode(payment?.Reason ?? "Unknown reason.") + " Nothing was charged.");
                    return;
                }

                // ----------  4. RE-CHECK STOCK (someone may have bought it while the payment was processing) ----------
                // Known limitation (as before): two buyers can still both pass this check in the instant between
                // the refresh and SaveChanges(); a row lock or rowversion column would close that small window.
                var ids = lines.Select(l => l.ProductId).ToList();
                var products = db.Products.Where(p => ids.Contains(p.ProductId)).ToList();
                ((IObjectContextAdapter)db).ObjectContext.Refresh(RefreshMode.StoreWins, products);   // GetLines already loaded them: re-read stock
                var byId = products.ToDictionary(p => p.ProductId);

                foreach (var line in lines)
                {
                    Models.Product product;
                    if (!byId.TryGetValue(line.ProductId, out product) || !product.IsActive || line.Quantity > product.StockQuantity)
                    {
                        ShowAlert(null, "Sorry, " + HttpUtility.HtmlEncode(line.Name) +
                                  " sold out while your payment was being processed. No order was placed.");
                        return;
                    }
                }
                foreach (var line in lines)
                    byId[line.ProductId].StockQuantity -= line.Quantity;   // saved with the order below

                // ----------  5. SAVE THE ORDER (card details are NOT saved) ----------
                // Re-use the saved address when every field matches; otherwise a new one is saved with the order
                // (it then appears in the address book, and becomes the default if the user has none)
                var address = new AddressBookService(db, userId).ResolveForOrder(new AddressInput
                {
                    AddressLine1 = txtLine1.Text,
                    AddressLine2 = txtLine2.Text,
                    City = txtCity.Text,
                    State = txtState.Text,
                    Country = txtCountry.Text,
                    ZipCode = txtZip.Text
                });

                var order = new Order
                {
                    UserId = userId,
                    Address = address,                 // EF saves the address and links it automatically
                    OrderDate = DateTime.Now,
                    Status = "Paid",
                    // One OrderItem per line, at the price that was charged
                    OrderItems = lines.Select(l => new OrderItem { ProductId = l.ProductId, Quantity = l.Quantity, UnitPrice = l.UnitPrice }).ToList()
                };

                db.Orders.Add(order);
                db.SaveChanges();   // order + every stock decrement in one save, so a shipping failure won't lose a paid order

                cart.Clear();

                // ----------  6. REQUEST SHIPPING ----------
                string deliveryName = txtShipName.Text.Trim();
                try
                {
                    var request = ShipmentSummary.Build(order.OrderId, deliveryName, address,
                        lines.Select(l => new KeyValuePair<string, int>(l.Name, l.Quantity)));
                    var shipment = await ApiClient.CreateShipmentAsync(request);

                    order.TrackingNumber = shipment.TrackingNumber;
                    order.Status = "Shipped";
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    order.Status = ShippingRetry.AwaitingStatus;
                    db.SaveChanges();
                }

                // ----------  7. CONFIRMATION ----------
                Done = true;
                DoneOrderId = order.OrderId;
                DoneTotal = total;
                DoneShipped = order.Status == "Shipped";
                DoneTracking = DoneShipped ? order.TrackingNumber : null;
                DoneName = FirstName(deliveryName);
                ClearCardFields();
            }
        }

        /// <summary>"John Brown" → "John"; no name → the part of the email before "@".</summary>
        private string FirstName(string fullName)
        {
            string first = (fullName ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrEmpty(first)) return first;
            string email = User.Identity.GetUserName() ?? "";
            int at = email.IndexOf('@');
            return at > 0 ? email.Substring(0, at) : email;
        }

        /// <summary>Card fields are never kept after an attempt.</summary>
        private void ClearCardFields()
        {
            txtFullName.Text = txtCardNumber.Text = txtExpiry.Text = txtCvv.Text = "";
        }

        /// <summary>Shows what CartService changed, and asks the customer to review before paying.</summary>
        private void ShowNotices(IList<string> notices)
        {
            string items = string.Concat(notices.Select(n => "<li>" + HttpUtility.HtmlEncode(n) + "</li>"));
            ShowAlert("Your cart changed.", " Please review your order before placing it. Nothing was charged.<ul>" + items + "</ul>");
        }

        /// <summary>
        /// Shows the pink alert at the top of the form and clears the card fields (the address is kept).
        /// <paramref name="title"/> is encoded here; <paramref name="html"/> must already be encoded by the caller.
        /// </summary>
        private void ShowAlert(string title, string html)
        {
            litAlert.Text = (title != null ? "<strong>" + HttpUtility.HtmlEncode(title) + "</strong>" : "") + html;
            pnlAlert.Visible = true;
            ClearCardFields();
        }
    }
}
