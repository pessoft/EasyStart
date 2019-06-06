namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCityIdReviewProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductReviews", "CityId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductReviews", "CityId");
        }
    }
}
