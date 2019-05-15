namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "Rating", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "Rating");
        }
    }
}
