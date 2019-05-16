namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddProductType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "ProductType", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProductModels", "ProductType");
        }
    }
}
