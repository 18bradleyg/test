using System;
using System.Web;
using System.Web.UI;

namespace eStoreJA
{
    /// <summary>
    /// Friendly error page. Web.config (customErrors) sends every unhandled error and
    /// every 404/403 here. It never shows exception details to the visitor.
    /// </summary>
    public partial class Error : Page
    {
        protected int StatusCode = 500;
        protected string Heading = "Something went wrong";
        protected string Message = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Work out which error brought the visitor here (ResponseRewrite keeps the last error available).
            var lastError = Server.GetLastError();
            var httpError = lastError as HttpException;

            int code;
            if (httpError != null)
            {
                StatusCode = httpError.GetHttpCode();
            }
            else if (lastError != null)
            {
                StatusCode = 500;   // any other unhandled exception
            }
            else if (int.TryParse(Request.QueryString["code"], out code) && (code == 400 || code == 403 || code == 404))
            {
                // Sent by IIS (Web.config httpErrors) or Global.asax: whitelisted so the URL can't fake other codes
                StatusCode = code;
            }
            else if (Response.StatusCode >= 400)
            {
                StatusCode = Response.StatusCode;
            }
            else
            {
                StatusCode = 404;   // opened /Error directly: there's no real page here
            }

            switch (StatusCode)
            {
                case 400:
                    Heading = "We couldn't process that request";
                    Message = "Something in the link or form wasn't allowed (for example < or > characters). Go back and try again.";
                    break;
                case 404:
                    Heading = "We can't find that page";
                    Message = "The page you're looking for doesn't exist or has moved. Head back home and try again.";
                    break;
                case 401:
                case 403:
                    Heading = "You don't have access to that page";
                    Message = "Please log in with an account that is allowed to view this page.";
                    break;
                default:
                    StatusCode = 500;
                    Heading = "Something went wrong on our side";
                    Message = "Please try again in a moment. If you were placing an order, check your order history before ordering again so you are not charged twice.";
                    break;
            }

            Title = Heading;

            // Send the real status code so browsers and search engines know this was an error page.
            Response.StatusCode = StatusCode;
            Response.TrySkipIisCustomErrors = true;
        }
    }
}
