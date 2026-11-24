namespace CreditCard.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cardholders",
                c => new
                    {
                        CardholderId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.CardholderId);
            
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        CardId = c.Int(nullable: false, identity: true),
                        CardholderId = c.Int(nullable: false),
                        CardNumber = c.String(nullable: false, maxLength: 16),
                        ExpiryDate = c.String(nullable: false, maxLength: 5),
                        Cvv = c.String(nullable: false, maxLength: 4),
                        AvailableBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CardId)
                .ForeignKey("dbo.Cardholders", t => t.CardholderId, cascadeDelete: true)
                .Index(t => t.CardholderId);
            
            CreateTable(
                "dbo.DeclinedTransactions",
                c => new
                    {
                        DeclinedTransactionId = c.Int(nullable: false, identity: true),
                        CardId = c.Int(),
                        AttemptedCardNumber = c.String(maxLength: 20),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(nullable: false, maxLength: 200),
                        AttemptDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeclinedTransactionId)
                .ForeignKey("dbo.Cards", t => t.CardId)
                .Index(t => t.CardId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        CardId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionDate = c.DateTime(nullable: false),
                        Status = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Cards", t => t.CardId, cascadeDelete: true)
                .Index(t => t.CardId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "CardId", "dbo.Cards");
            DropForeignKey("dbo.DeclinedTransactions", "CardId", "dbo.Cards");
            DropForeignKey("dbo.Cards", "CardholderId", "dbo.Cardholders");
            DropIndex("dbo.Transactions", new[] { "CardId" });
            DropIndex("dbo.DeclinedTransactions", new[] { "CardId" });
            DropIndex("dbo.Cards", new[] { "CardholderId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.DeclinedTransactions");
            DropTable("dbo.Cards");
            DropTable("dbo.Cardholders");
        }
    }
}
