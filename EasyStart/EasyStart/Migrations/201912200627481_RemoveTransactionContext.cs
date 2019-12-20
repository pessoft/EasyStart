namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTransactionContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CashbackTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionType = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Money = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PartnersTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionType = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        ReferralId = c.Int(nullable: false),
                        Money = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefundCashbackTransactionModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CashbackTransactionId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RefundCashbackTransactionModels");
            DropTable("dbo.PartnersTransactions");
            DropTable("dbo.CashbackTransactions");
        }
    }
}
