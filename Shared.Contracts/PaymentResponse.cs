using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    /// <summary>
    /// Returned by CreditCard.Api after checking a payment.
    /// Always sent with HTTP 200, whether approved or declined.
    /// </summary>
    public class PaymentResponse
    {
        /// <summary>"approved" or "declined".</summary>
        public string Status { get; set; }

        /// <summary>Why the payment was declined. Null when approved.</summary>
        public string Reason { get; set; }
    }
}