using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CreditCard.Api.Models
{
    /// <summary>An approved payment charged to a card.</summary>
    public class Transaction
    {
        /// <summary>Primary key.</summary>
        public int TransactionId { get; set; }

        /// <summary>The card that was charged.</summary>
        public int CardId { get; set; }

        /// <summary>Amount charged.</summary>
        public decimal Amount { get; set; }

        /// <summary>When the payment was approved (UTC).</summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>Transaction status, e.g. "approved".</summary>
        [StringLength(20)]
        public string Status { get; set; }

        /// <summary>The card that was charged.</summary>
        public virtual Card Card { get; set; }


    }
}