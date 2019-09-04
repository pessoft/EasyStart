namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSettingIdAreaDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaDeliveryModels", "deliverySettingId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AreaDeliveryModels", "deliverySettingId");
        }
    }
}
