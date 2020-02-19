namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMainPriceDelivery : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DeliverySettingModels", "PriceDelivery");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeliverySettingModels", "PriceDelivery", c => c.Double(nullable: false));
        }
    }
}
