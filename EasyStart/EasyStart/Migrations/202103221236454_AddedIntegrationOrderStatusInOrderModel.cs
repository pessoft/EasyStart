namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIntegrationOrderStatusInOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "IntegrationOrderStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "IntegrationOrderStatus");
        }
    }
}
