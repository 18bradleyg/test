using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CreditCard.Api.Models
{
    /// <summary>A person who owns one or more credit cards.</summary>
    public class Cardholder
    {
        /// <summary>Primary key.</summary>
        public int CardholderId { get; set; }

        /// <summary>Name as printed on the card; checked (case-insensitive) during authorization.</summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>Contact email address.</summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        /// <summary>Cards owned by this cardholder.</summary>
        public virtual ICollection<Card> Cards { get; set; }
            // Navigation property: A cardholder can have multiple cards, so we use a collection to represent that relationship.

    }
}