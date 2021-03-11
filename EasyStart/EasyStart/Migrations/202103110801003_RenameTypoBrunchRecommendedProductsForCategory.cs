namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTypoBrunchRecommendedProductsForCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecommendedProductModels", "BranchId", c => c.Int(nullable: false));
            DropColumn("dbo.RecommendedProductModels", "BrunchId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RecommendedProductModels", "BrunchId", c => c.Int(nullable: false));
            DropColumn("dbo.RecommendedProductModels", "BranchId");
        }
    }
}
