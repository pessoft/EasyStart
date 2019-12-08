namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeStockIdToStockIdsInOrderModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderStockApplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        StockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.OrderModels", "StockId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "StockId", c => c.Int(nullable: false));
            DropTable("dbo.OrderStockApplies");
        }
    }
}
