namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeliverySettingIdAreaDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaDeliveryModels", "DeliverySettingId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AreaDeliveryModels", "DeliverySettingId");
        }
    }
}
