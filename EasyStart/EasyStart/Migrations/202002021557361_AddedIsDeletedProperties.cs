namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDeletedProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SettingModels", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SettingModels", "IsDeleted");
            DropColumn("dbo.DeliverySettingModels", "IsDeleted");
        }
    }
}
