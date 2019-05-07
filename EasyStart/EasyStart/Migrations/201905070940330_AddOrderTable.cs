namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        DeliveryType = c.Int(nullable: false),
                        Street = c.String(),
                        HomeNumber = c.String(),
                        EntranceNumber = c.String(),
                        ApartamentNumber = c.String(),
                        Level = c.String(),
                        IntercomCode = c.String(),
                        BuyType = c.Int(nullable: false),
                        Comment = c.String(),
                        ProductCountJSON = c.String(),
                        Discount = c.Double(nullable: false),
                        CashBack = c.Double(nullable: false),
                        NeedCashBack = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OrderModels");
        }
    }
}
