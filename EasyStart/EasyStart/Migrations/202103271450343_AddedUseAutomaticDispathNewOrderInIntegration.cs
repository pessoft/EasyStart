namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUseAutomaticDispathNewOrderInIntegration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IntegrationSystemModels", "UseAutomaticDispatch", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IntegrationSystemModels", "UseAutomaticDispatch");
        }
    }
}
