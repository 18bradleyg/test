using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace eStoreJA
{
    /// <summary>
    /// Base class for pages that require a logged-in user.
    /// Pages inherit SecurePage instead of Page to be protected automatically.
    /// </summary>
    public class SecurePage : Page
    {
        // OnInit runs before Page_Load, so the user is redirected before any page code runs
        protected override void OnInit(EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Send them to Login, and remember where they were trying to go
                string returnUrl = HttpUtility.UrlEncode(Request.RawUrl);
                Response.Redirect("~/Account/Login?ReturnUrl=" + returnUrl, true);
            }

            base.OnInit(e);
        }
    }
}