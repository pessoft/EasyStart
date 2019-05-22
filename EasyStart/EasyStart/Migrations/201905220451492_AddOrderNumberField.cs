namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddOrderNumberField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "OrderNumber", c => c.Int(nullable: false));
            AddColumn("dbo.ProductModels", "OrderNumber", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProductModels", "OrderNumber");
            DropColumn("dbo.CategoryModels", "OrderNumber");
        }
    }
}
