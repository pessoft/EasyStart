namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientIdInReviewModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductReviews", "ClientId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductReviews", "ClientId");
        }
    }
}
