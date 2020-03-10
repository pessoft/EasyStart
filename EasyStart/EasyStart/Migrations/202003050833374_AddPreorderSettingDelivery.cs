namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPreorderSettingDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "MaxPreorderPeriod", c => c.Int(nullable: false));
            AddColumn("dbo.DeliverySettingModels", "MinTimeProcessingOrder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "MinTimeProcessingOrder");
            DropColumn("dbo.DeliverySettingModels", "MaxPreorderPeriod");
        }
    }
}
