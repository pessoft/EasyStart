namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSettingIdAreaDelivery : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AreaDeliveryModels", "DeliverySettingId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AreaDeliveryModels", "DeliverySettingId", c => c.Int(nullable: false));
        }
    }
}
