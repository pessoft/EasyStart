namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderIdExternal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "IntegrationOrderId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "IntegrationOrderId");
        }
    }
}
