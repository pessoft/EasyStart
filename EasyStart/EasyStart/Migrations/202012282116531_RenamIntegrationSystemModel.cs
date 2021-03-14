namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamIntegrationSystemModel : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.IntegrationSystemModals", newName: "IntegrationSystemModels");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.IntegrationSystemModels", newName: "IntegrationSystemModals");
        }
    }
}
