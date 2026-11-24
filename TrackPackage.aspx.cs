using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using eStoreJA.Services;
using Shared.Contracts;

namespace eStoreJA
{
    public partial class TrackPackage : SecurePage
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-teal";
            if (!IsPostBack)
            {
                string tracking = Request.QueryString["tracking"];
                if (!string.IsNullOrWhiteSpace(tracking))
                {
                    txtTrackingNumber.Text = tracking.Trim();
                    await LoadTrackingAsync(tracking.Trim());
                }
            }
        }

        protected async void btnTrack_Click(object sender, EventArgs e)
        {
            pnlResult.Visible = false;
            string tracking = (txtTrackingNumber.Text ?? "").Trim();
            if (tracking.Length == 0)
            {
                ShowMessage("Please enter a tracking number.", true);
                return;
            }

            await LoadTrackingAsync(tracking);
        }

        private async Task LoadTrackingAsync(string trackingNumber)
        {
            try
            {
                TrackingResponse result = await ApiClient.TrackShipmentAsync(trackingNumber);
                if (result == null)
                {
                    ShowMessage("We couldn't find a shipment with that tracking number. Check it and try again.", true);
                    return;
                }

                lblTracking.Text = HttpUtility.HtmlEncode(result.TrackingNumber);
                lblStatus.Text = HttpUtility.HtmlEncode(result.Status);
                lblEstimated.Text = result.EstimatedDelivery.ToString("dd MMM yyyy");
                lblActual.Text = result.ActualDelivery.HasValue
                    ? result.ActualDelivery.Value.ToString("dd MMM yyyy")
                    : "Not delivered yet";
                pnlResult.Visible = true;
                ShowMessage("", false);
            }
            catch (Exception)
            {
                ShowMessage("The shipping service is temporarily unavailable. Please try again later.", true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = HttpUtility.HtmlEncode(message);
            lblMessage.CssClass = isError ? "d-block mt-3 text-danger" : "d-block mt-3 text-success";
        }
    }
}
