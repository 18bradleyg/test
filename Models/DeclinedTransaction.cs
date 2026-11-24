using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CreditCard.Api.Models
{
    /// <summary>A payment attempt that was declined, kept for auditing.</summary>
    public class DeclinedTransaction
    {
        /// <summary>Primary key.</summary>
        public int DeclinedTransactionId { get; set; }

        /// <summary>The matching card, or null if the card number was not found.</summary>
        public int? CardId { get; set; } // Nullable in case the card number doesn't exist

        /// <summary>Masked card number (last 4 digits only).</summary>
        [StringLength(20)]
        public string AttemptedCardNumber { get; set; }

        /// <summary>Amount that was requested.</summary>
        public decimal Amount { get; set; }

        /// <summary>Why the payment was declined, e.g. "Insufficient funds".</summary>
        [Required]
        [StringLength(200)]
        public string Reason { get; set; }

        /// <summary>When the attempt was made (UTC).</summary>
        public DateTime AttemptDate { get; set; }

        /// <summary>The matching card, if any.</summary>
        public virtual Card Card { get; set; }


    }
}