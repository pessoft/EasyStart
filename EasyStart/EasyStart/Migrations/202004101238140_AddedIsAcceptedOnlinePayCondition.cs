namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsAcceptedOnlinePayCondition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "IsAcceptedOnlinePayCondition", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "IsAcceptedOnlinePayCondition");
        }
    }
}
