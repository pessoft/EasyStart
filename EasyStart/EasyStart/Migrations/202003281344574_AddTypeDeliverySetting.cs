namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTypeDeliverySetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "IsDelivery", c => c.Boolean(nullable: true));
            AddColumn("dbo.DeliverySettingModels", "IsTakeYourSelf", c => c.Boolean(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "IsTakeYourSelf");
            DropColumn("dbo.DeliverySettingModels", "IsDelivery");
        }
    }
}
