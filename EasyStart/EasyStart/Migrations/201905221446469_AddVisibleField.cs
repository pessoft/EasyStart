namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddVisibleField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "Visible", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductModels", "Visible", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProductModels", "Visible");
            DropColumn("dbo.CategoryModels", "Visible");
        }
    }
}
