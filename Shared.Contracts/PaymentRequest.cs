using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    // Shared.Contracts/PaymentRequest.cs
    // What the website sends to the Credit Card API
    public class PaymentRequest
    {
        public string FullName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }   // format "MM/YY"
        public string Cvv { get; set; }
        public decimal Amount { get; set; }
    }
}
