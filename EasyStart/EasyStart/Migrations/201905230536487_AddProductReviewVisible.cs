namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddProductReviewVisible : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductReviews", "Visible", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProductReviews", "Visible");
        }
    }
}
