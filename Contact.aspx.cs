using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

namespace eStoreJA
{
    /// <summary>
    /// Contact us page (dark theme) with a message form. There is no email backend yet, so a valid
    /// message is not sent or stored – the page just shows the "Message sent." confirmation.
    /// </summary>
    public partial class Contact : Page
    {
        // Same rule as the validator: "ORD-1042" or "1042"
        private static readonly Regex OrderNumberPattern = new Regex(@"^(ORD-)?\d+$", RegexOptions.IgnoreCase);

        /// <summary>Dark theme; on the first visit pre-fills the email, topic and order number.</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-dark";

            if (!IsPostBack)
            {
                // Signed in: UserName is the email in this app; the box stays editable
                if (User.Identity.IsAuthenticated)
                {
                    txtEmail.Text = User.Identity.GetUserName();
                }

                // ?topic=delivery – only the five known values; anything else keeps the default ("An order")
                ListItem topic = rblTopic.Items.FindByValue((Request.QueryString["topic"] ?? "").Trim().ToLowerInvariant());
                if (topic != null)
                {
                    rblTopic.ClearSelection();
                    topic.Selected = true;
                }

                // ?order=ORD-1042 – lets Order History link to "Contact us about this order" later.
                // Only a well-formed number is accepted (the TextBox HTML-encodes it anyway).
                string order = (Request.QueryString["order"] ?? "").Trim();
                if (order.Length <= 20 && OrderNumberPattern.IsMatch(order))
                {
                    txtOrder.Text = order.ToUpperInvariant();
                }
            }
        }

        /// <summary>Checks the form again on the server, then shows the confirmation instead of the form.</summary>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;   // client-side checks can be bypassed

            // TODO: send the message (email service or ContactMessages table).
            //       Use txtName.Text, txtEmail.Text, rblTopic.SelectedValue, txtOrder.Text and txtMessage.Text.

            pnlForm.Visible = false;
            pnlSent.Visible = true;
        }
    }
}
