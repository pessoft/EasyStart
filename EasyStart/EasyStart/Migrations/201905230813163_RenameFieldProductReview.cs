namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameFieldProductReview : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductReviews", "ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.ProductReviews", "PorudctId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductReviews", "PorudctId", c => c.Int(nullable: false));
            DropColumn("dbo.ProductReviews", "ProductId");
        }
    }
}
