namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOnlinePayInDeliverySetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "PayOnline", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "PayOnline");
        }
    }
}
