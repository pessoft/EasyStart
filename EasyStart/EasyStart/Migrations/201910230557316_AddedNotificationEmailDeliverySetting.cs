namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNotificationEmailDeliverySetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "NotificationEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "NotificationEmail");
        }
    }
}
