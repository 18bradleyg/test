using System.Data.Entity.Migrations;
using CreditCard.Api.Models;

namespace CreditCard.Api.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<CreditCard.Api.Data.CreditCardDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CreditCard.Api.Data.CreditCardDbContext context)
        {
            var holder1 = new Cardholder
            {
                CardholderId = 1,
                FullName = "Marcus Campbell",
                Email = "marcus.campbell@example.com"
            };

            var holder2 = new Cardholder
            {
                CardholderId = 2,
                FullName = "Brianna Reid",
                Email = "brianna.reid@example.com"
            };

            context.Cardholders.AddOrUpdate(
                h => h.Email,
                holder1,
                holder2
            );
            context.SaveChanges();

            var dbHolder1 = context.Cardholders.Find(holder1.CardholderId) ?? holder1;
            var dbHolder2 = context.Cardholders.Find(holder2.CardholderId) ?? holder2;

            context.Cards.AddOrUpdate(
                c => c.CardNumber,
                new Card
                {
                    CardholderId = dbHolder1.CardholderId,
                    CardNumber = "4111111111111111", // Test 1: Approves
                    ExpiryDate = "12/28",
                    Cvv = "123",
                    AvailableBalance = 50000.00m,
                    IsActive = true
                },
                new Card
                {
                    CardholderId = dbHolder1.CardholderId,
                    CardNumber = "4222222222222222", // Test 2: Low balance
                    ExpiryDate = "10/27",
                    Cvv = "456",
                    AvailableBalance = 10.00m,
                    IsActive = true
                },
                new Card
                {
                    CardholderId = dbHolder2.CardholderId,
                    CardNumber = "4333333333333333", // Test 3: Wrong CVV (stored as 789)
                    ExpiryDate = "08/29",
                    Cvv = "789",
                    AvailableBalance = 25000.00m,
                    IsActive = true
                }
            );
            context.SaveChanges();
        }
    }
}