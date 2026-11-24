using CreditCard.Api.Data;
using CreditCard.Api.Models;
using Shared.Contracts;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace CreditCard.Api.Controllers
{
    /// <summary>
    /// Credit card authorization endpoint used by eStoreJA at checkout.
    /// Validates the card details, checks the balance, and records approved or declined transactions.
    /// </summary>
    public class PaymentsController : ApiController
    {
        /// <summary>
        /// Authorizes a payment: verifies card number, cardholder name, expiry date, CVV and available balance,
        /// then deducts the amount on success.
        /// </summary>
        /// <param name="request">The card details and amount submitted by the store.</param>
        /// <returns>A response with status "approved", or "declined" plus the reason.</returns>
        [HttpPost]
        [Route("api/payments/authorize")]
        public PaymentResponse Authorize(PaymentRequest request)
        {
            using (var db = new CreditCardDbContext())
            {
                if (request == null || string.IsNullOrWhiteSpace(request.CardNumber))
                {
                    return Decline(db, null, "", request?.Amount ?? 0m, "Invalid payment request");
                }

                string cleanCardNumber = request.CardNumber.Replace(" ", "").Replace("-", "").Trim();
                string cleanName = (request.FullName ?? "").Trim();
                string cleanExpiry = (request.ExpiryDate ?? "").Trim();
                string cleanCvv = (request.Cvv ?? "").Trim();

                if (request.Amount <= 0)
                {
                    return Decline(db, null, cleanCardNumber, request.Amount, "Invalid transaction amount");
                }

                // 1. Find card + cardholder
                var card = db.Cards
                    .Include(c => c.Cardholder)
                    .FirstOrDefault(c => c.CardNumber == cleanCardNumber);

                if (card == null)
                {
                    return Decline(db, null, cleanCardNumber, request.Amount, "Invalid card number");
                }

                if (!card.IsActive)
                {
                    return Decline(db, card.CardId, cleanCardNumber, request.Amount, "Card is inactive");
                }

                // 2. Verify cardholder name, expiry date, and CVV
                if (!string.Equals(card.Cardholder.FullName.Trim(), cleanName, StringComparison.OrdinalIgnoreCase))
                {
                    return Decline(db, card.CardId, cleanCardNumber, request.Amount, "Cardholder name mismatch");
                }

                if (card.ExpiryDate.Trim() != cleanExpiry)
                {
                    return Decline(db, card.CardId, cleanCardNumber, request.Amount, "Invalid expiry date");
                }

                if (IsCardExpired(cleanExpiry))
                {
                    return Decline(db, card.CardId, cleanCardNumber, request.Amount, "Card has expired");
                }

                if (card.Cvv.Trim() != cleanCvv)
                {
                    return Decline(db, card.CardId, cleanCardNumber, request.Amount, "Invalid CVV");
                }

                // 3. Check available balance
                if (card.AvailableBalance < request.Amount)
                {
                    return Decline(db, card.CardId, cleanCardNumber, request.Amount, "Insufficient funds");
                }

                // 4. Approve: deduct balance and record approved transaction
                card.AvailableBalance -= request.Amount;

                db.Transactions.Add(new Transaction
                {
                    CardId = card.CardId,
                    Amount = request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Status = "approved"
                });

                db.SaveChanges();

                return new PaymentResponse
                {
                    Status = "approved",
                    Reason = null
                };
            }
        }

        /// <summary>Records a declined attempt (with a masked card number) and returns a "declined" response.</summary>
        private PaymentResponse Decline(CreditCardDbContext db, int? cardId, string cardNumber, decimal amount, string reason)
        {
            db.DeclinedTransactions.Add(new DeclinedTransaction
            {
                CardId = cardId,
                AttemptedCardNumber = MaskCardNumber(cardNumber),
                Amount = amount,
                Reason = reason,
                AttemptDate = DateTime.UtcNow
            });

            db.SaveChanges();

            return new PaymentResponse
            {
                Status = "declined",
                Reason = reason
            };
        }

        /// <summary>Keeps only the last 4 digits, e.g. ************1111, so full card numbers are never stored.</summary>
        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length <= 4) return cardNumber;
            return new string('*', cardNumber.Length - 4) + cardNumber.Substring(cardNumber.Length - 4);
        }

        /// <summary>Returns true if the MM/yy expiry is unparseable or its month has already ended.</summary>
        private bool IsCardExpired(string expiryMmYy)
        {
            if (!DateTime.TryParseExact(expiryMmYy, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
            {
                return true;
            }

            // Cards remain valid through the last day of their expiration month
            var lastDayOfExpiryMonth = new DateTime(parsed.Year, parsed.Month, DateTime.DaysInMonth(parsed.Year, parsed.Month), 23, 59, 59);
            return DateTime.UtcNow > lastDayOfExpiryMonth;
        }
    }
}