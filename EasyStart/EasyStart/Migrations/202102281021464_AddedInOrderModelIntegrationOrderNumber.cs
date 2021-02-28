namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInOrderModelIntegrationOrderNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "IntegrationOrderNumber", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "IntegrationOrderNumber");
        }
    }
}
