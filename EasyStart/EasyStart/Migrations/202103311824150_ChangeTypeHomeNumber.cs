namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeHomeNumber : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SettingModels", "HomeNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SettingModels", "HomeNumber", c => c.Int(nullable: false));
        }
    }
}
