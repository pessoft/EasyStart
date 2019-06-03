namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddOrderStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "OrderStatus", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.OrderModels", "OrderStatus");
        }
    }
}
