using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CreditCard.Api.Models;

namespace CreditCard.Api.Data
{
    /// <summary>Entity Framework context for the credit card database.</summary>
    public class CreditCardDbContext : DbContext
    {
        /// <summary>Uses the "CreditCardDbContext" connection string from Web.config.</summary>
        public CreditCardDbContext() : base("CreditCardDbContext")
        {
        }

        /// <summary>Issued credit cards.</summary>
        public DbSet<Card> Cards { get; set; }
        /// <summary>Card owners.</summary>
        public DbSet<Cardholder> Cardholders { get; set; }
        /// <summary>Approved transactions.</summary>
        public DbSet<Transaction> Transactions { get; set; }
        /// <summary>Declined authorization attempts.</summary>
        public DbSet<DeclinedTransaction> DeclinedTransactions { get; set; }
    }
}