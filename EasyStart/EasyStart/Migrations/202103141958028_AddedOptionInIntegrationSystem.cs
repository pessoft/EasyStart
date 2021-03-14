namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOptionInIntegrationSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IntegrationSystemModels", "Options", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IntegrationSystemModels", "Options");
        }
    }
}
