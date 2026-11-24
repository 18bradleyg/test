using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace eStoreJA
{
    /// <summary>
    /// Base class for every page in /Admin. Only logged-in users in the "Admin" role get in.
    /// (Web.config has a matching &lt;location path="Admin"&gt; rule as a second line of defence.)
    /// Also holds small display helpers shared by the admin pages.
    /// </summary>
    public class AdminPage : Page
    {
        /// <summary>Stock at or below this (but above 0) counts as "Low stock".</summary>
        public const int LowStockLimit = 5;

        /// <summary>Redirects anonymous users to Login and non-admins to the home page, before any page code runs.</summary>
        protected override void OnInit(EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Same as SecurePage: remember where they were going
                string returnUrl = HttpUtility.UrlEncode(Request.RawUrl);
                Response.Redirect("~/Account/Login?ReturnUrl=" + returnUrl, true);
            }
            else if (!User.IsInRole(AppRoles.Admin))
            {
                // Logged in but not an admin: send them back to the store
                Response.Redirect("~/", true);
            }

            base.OnInit(e);
        }

        private const string FlashKey = "AdminFlash";

        /// <summary>Remembers a one-time success message to show on the next admin page (survives a redirect).</summary>
        protected void SetFlash(string message)
        {
            Session[FlashKey] = message;
        }

        /// <summary>Returns the one-time message (if any) and forgets it.</summary>
        protected string TakeFlash()
        {
            var message = Session[FlashKey] as string;
            Session.Remove(FlashKey);
            return message;
        }

        /// <summary>Resolves local "~/" image paths; full links (e.g. S3) are used as they are.</summary>
        protected string ImageSrc(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";
            return url.StartsWith("~") ? ResolveUrl(url) : url;
        }

        /// <summary>Formats money the way the store does in lists, e.g. J$12,000.</summary>
        protected static string Money(decimal value)
        {
            return string.Format("J${0:N0}", value);
        }

        /// <summary>Display SKU for a product, e.g. SKU-07.</summary>
        protected static string Sku(int productId)
        {
            return "SKU-" + productId.ToString("00");
        }

        /// <summary>Two-letter tile for a product without a picture, e.g. "Bluetooth Headphones" → "BH".</summary>
        protected static string Initials(string name)
        {
            var words = (name ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string letters = new string(words.Take(2).Select(w => char.ToUpperInvariant(w[0])).ToArray());
            return letters.Length > 0 ? letters : "?";
        }

        /// <summary>CSS modifier and label for a product's stock/visibility pill.</summary>
        protected static string StockPill(int stock, bool isActive)
        {
            if (!isActive) return "<span class=\"pill pill-hidden\">Hidden</span>";
            if (stock <= 0) return "<span class=\"pill pill-out\">Out of stock</span>";
            if (stock <= LowStockLimit) return "<span class=\"pill pill-warn\"><i></i>Low stock</span>";
            return "<span class=\"pill pill-ok\"><i></i>In stock</span>";
        }

        /// <summary>Id of the signed-in admin ("me" in the safety rules).</summary>
        protected string CurrentUserId
        {
            get { return Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity); }
        }

        /// <summary>"Today 18:24" for today's orders, otherwise "21 Nov 2026".</summary>
        protected static string Placed(DateTime when)
        {
            return when.Date == DateTime.Today ? "Today " + when.ToString("HH:mm") : when.ToString("d MMM yyyy");
        }

        /// <summary>Join date in server time, or "—" for accounts made before it was tracked.</summary>
        protected static string Joined(DateTime? createdAtUtc)
        {
            return createdAtUtc.HasValue ? createdAtUtc.Value.ToLocalTime().ToString("d MMM yyyy") : "—";
        }

        /// <summary>Audit-log time (stored UTC) in server time.</summary>
        protected static string LogTime(DateTime utc)
        {
            return DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime().ToString("d MMM yyyy, HH:mm");
        }

        /// <summary>Two letters for an avatar from an email, e.g. "john@…" → "JO".</summary>
        protected static string EmailInitials(string email)
        {
            string name = (email ?? "?").Split('@')[0];
            return name.Substring(0, Math.Min(2, name.Length)).ToUpperInvariant();
        }

        /// <summary>Role pills: Admin navy/white, Customer teal, custom roles outlined (all encoded).</summary>
        protected static string RolePills(System.Collections.Generic.IEnumerable<string> roles)
        {
            var sb = new System.Text.StringBuilder();
            foreach (string r in roles.OrderBy(r => r == AppRoles.Admin ? 0 : r == AppRoles.Customer ? 1 : 2).ThenBy(r => r))
            {
                string css = r == AppRoles.Admin ? "pill pill-out" : r == AppRoles.Customer ? "pill pill-ok" : "pill pill-role";
                sb.Append("<span class=\"").Append(css).Append("\">").Append(HttpUtility.HtmlEncode(r)).Append("</span> ");
            }
            return sb.Length > 0 ? sb.ToString() : "<span class=\"pill pill-hidden\">No role</span>";
        }

        /// <summary>Human sentence for an audit row, e.g. "Role assigned: Admin".</summary>
        protected static string DescribeLog(eStoreJA.Models.AdminAuditLog log)
        {
            string text;
            switch (log.Action)
            {
                case "RoleAssigned": text = "Role assigned: " + log.RoleName; break;
                case "RoleRevoked": text = "Role removed: " + log.RoleName; break;
                case "UserCreated": text = "Account created"; break;
                case "UserDisabled": text = "Account disabled"; break;
                case "UserEnabled": text = "Account enabled"; break;
                case "UserDeleted": text = "Account deleted"; break;
                default: text = log.Action + (log.RoleName != null ? ": " + log.RoleName : ""); break;
            }
            return string.IsNullOrEmpty(log.Details) ? text : text + " (" + log.Details + ")";
        }

        /// <summary>Active / Disabled pill for an account.</summary>
        protected static string AccountPill(bool disabled)
        {
            return disabled ? "<span class=\"pill pill-warn\"><i></i>Disabled</span>" : "<span class=\"pill pill-ok\"><i></i>Active</span>";
        }

        /// <summary>Status pill for an order (Shipped = teal, anything not shipped = pink).</summary>
        protected static string OrderPill(string status)
        {
            string text = HttpUtility.HtmlEncode(status ?? "");
            return status == "Shipped"
                ? "<span class=\"pill pill-ok\"><i></i>" + text + "</span>"
                : "<span class=\"pill pill-warn\"><i></i>" + text + "</span>";
        }
    }
}
