namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDelvieryPriceOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "DeliveryPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "DeliveryPrice");
        }
    }
}
