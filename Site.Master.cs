using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

namespace eStoreJA
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
            Page.PreRenderComplete += master_Page_PreRenderComplete;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        /// <summary>True for users in the Admin role (shows the Admin links; the /Admin pages check this too).</summary>
        protected bool IsAdmin { get; private set; }

        /// <summary>The signed-in user's first name for "Hi, John" (from the cookie claim – no database query).</summary>
        protected string FirstName { get; private set; } = "";

        /// <summary>First letter of <see cref="FirstName"/> for the round initial in the account menu.</summary>
        protected string FirstInitial
        {
            get { return FirstName.Length > 0 ? FirstName.Substring(0, 1).ToUpperInvariant() : "?"; }
        }

        /// <summary>Category names for the search chips and footer (cached, see <see cref="GetCategoryNames"/>).</summary>
        protected List<string> CategoryNames { get; private set; }

        private const string CategoryCacheKey = "SiteMaster.CategoryNames";

        /// <summary>
        /// False on form-only pages (Log in / Register, Forgot password): the header, footer and skip link
        /// are not rendered. Set it from the content page's Page_Load (that runs before this Page_Load).
        /// </summary>
        public bool ShowSiteChrome { get; set; } = true;

        /// <summary>
        /// Extra class(es) for &lt;body&gt;. Lets a page switch the master to a dark theme
        /// ("page-dark": navy page, white logo, light navbar outlines). Set it in the page's Page_Load.
        /// </summary>
        public string BodyCssClass { get; set; } = "";

        /// <summary>Items in the cart (sum of quantities) for the header badge.</summary>
        protected int CartCount { get; private set; }

        /// <summary>"Cart, 3 items" (or just "Cart" when empty) for the cart link.</summary>
        protected string CartAriaLabel
        {
            get { return CartCount == 0 ? "Cart" : "Cart, " + CartCount + (CartCount == 1 ? " item" : " items"); }
        }

        /// <summary>The add-to-cart result to show as a toast on this page view (set by Shop / Home, then redirected here).</summary>
        protected Services.AddResult CartToast { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            phSkipLink.Visible = phHeader.Visible = phFooter.Visible = ShowSiteChrome;

            // The Admin link (account dropdown) is only for users in the Admin role (the pages check this too)
            IsAdmin = Context.User.IsInRole(AppRoles.Admin);

            if (Context.User.Identity.IsAuthenticated)
            {
                FirstName = Context.User.Identity.GetFirstName();
            }

            // Same list for the search "Browse" chips and the footer Shop column.
            // Bound on every request (ViewState off) because they are plain links.
            CategoryNames = GetCategoryNames();
            foreach (var repeater in new[] { rptSearchCategories, rptFooterCategories })
            {
                repeater.DataSource = CategoryNames;
                repeater.DataBind();
            }
        }

        /// <summary>
        /// Reads the cart count and any pending toast. Runs at the page's PreRenderComplete (not Load / PreRender)
        /// so the badge reflects this request's button clicks, including async ones such as Checkout's Place order.
        /// </summary>
        protected void master_Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!ShowSiteChrome) return;   // no header on form-only pages

            CartToast = Services.CartService.TakeFlash(Context);
            try
            {
                using (var db = new Models.ApplicationDbContext())
                    CartCount = new Services.CartService(Context, db).Count();
            }
            catch (Exception)
            {
                CartCount = 0;   // the header must still render if the database can't be read
            }
        }

        /// <summary>
        /// All category names, A→Z. Cached for 10 minutes in HttpRuntime.Cache so the header and footer
        /// don't query the database on every page view. Returns an empty list if the database can't be read,
        /// so the header still renders (e.g. on a page that is already reporting another error).
        /// </summary>
        public static List<string> GetCategoryNames()   // public: the Shop page's department pills use it too
        {
            var cached = HttpRuntime.Cache[CategoryCacheKey] as List<string>;
            if (cached != null) return cached;

            List<string> names;
            try
            {
                using (var db = new Models.ApplicationDbContext())
                    names = db.Categories.OrderBy(c => c.Name).Select(c => c.Name).ToList();
            }
            catch (Exception)
            {
                return new List<string>();   // not cached: try again on the next request
            }

            HttpRuntime.Cache.Insert(CategoryCacheKey, names, null,
                DateTime.UtcNow.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration);
            return names;
        }

        /// <summary>
        /// "active" when <paramref name="appRelativePath"/> is the current page (case-insensitive, ".aspx" ignored,
        /// "~/" and "~/Default" both mean Home), otherwise "". Used for the pink current-page pill.
        /// </summary>
        protected string NavState(string appRelativePath)
        {
            return Normalize(Request.AppRelativeCurrentExecutionFilePath) == Normalize(appRelativePath) ? "active" : "";
        }

        /// <summary>aria-current="page" for the current page's link (a fixed string, safe to write raw).</summary>
        protected string AriaCurrent(string appRelativePath)
        {
            return NavState(appRelativePath) == "active" ? "aria-current=\"page\"" : "";
        }

        /// <summary>~/PlaceOrder.aspx, ~/placeorder and ~/PlaceOrder/ → "~/placeorder"; ~/ → "~/default".</summary>
        private static string Normalize(string path)
        {
            string p = (path ?? "~/").ToLowerInvariant();
            if (p.EndsWith(".aspx")) p = p.Substring(0, p.Length - 5);
            p = p.TrimEnd('/');
            return p == "~" || p == "" ? "~/default" : p;
        }

        /// <summary>The Shop page filtered to one category (same link the home page uses).</summary>
        protected string CategoryUrl(string categoryName)
        {
            return ResolveUrl("~/Shop?category=" + HttpUtility.UrlEncode(categoryName));
        }

        /// <summary>Two-letter tile code: "Home &amp; Living" → "HL", "Beauty" → "BE".</summary>
        public static string CategoryCode(string name)   // public: the home page hero menu uses it too
        {
            var words = (name ?? "").Split(new[] { ' ', '&' }, StringSplitOptions.RemoveEmptyEntries);
            string code = words.Length >= 2
                ? new string(new[] { words[0][0], words[1][0] })
                : (words.Length == 1 ? words[0].Substring(0, Math.Min(2, words[0].Length)) : "?");
            return code.ToUpperInvariant();
        }

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }

}