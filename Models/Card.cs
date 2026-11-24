using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CreditCard.Api.Models
{
    /// <summary>A credit card issued to a cardholder.</summary>
    public class Card
    {
         /// <summary>Primary key.</summary>
         public int CardId { get; set; }
         /// <summary>Owner of this card.</summary>
         public int CardholderId { get; set; }  // Foreign key to Cardholder

        /// <summary>16-digit card number, no spaces or dashes.</summary>
        [Required]
        [StringLength (16)]
        public string CardNumber { get; set; }

        /// <summary>Expiry date in MM/YY format.</summary>
        [Required]
        [StringLength(5)]
        public string ExpiryDate { get; set; } // Format "MM/YY"

        /// <summary>3 or 4 digit security code.</summary>
        [Required]
        [StringLength(4)]
        public string Cvv { get; set; }

        /// <summary>Credit still available; reduced by each approved transaction.</summary>
        public decimal AvailableBalance { get; set; }

        /// <summary>Inactive cards are always declined.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>The card's owner.</summary>
        public virtual Cardholder Cardholder { get; set; }
        /// <summary>Approved transactions on this card.</summary>
        public virtual ICollection<Transaction> Transactions { get; set; }
        /// <summary>Declined attempts on this card.</summary>
        public virtual ICollection<DeclinedTransaction> DeclinedTransactions { get; set; }



    }
}