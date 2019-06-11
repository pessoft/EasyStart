namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSoundNotify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "IsSoundNotify", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "IsSoundNotify");
        }
    }
}
