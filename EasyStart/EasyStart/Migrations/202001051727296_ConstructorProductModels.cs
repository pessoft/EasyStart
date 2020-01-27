namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConstructorProductModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConstructorCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        MaxCountIngredient = c.Int(nullable: false),
                        StyleTypeIngredient = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IngredientModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubCategoryId = c.Int(nullable: false),
                        Name = c.String(),
                        AdditionaInfo = c.String(),
                        Price = c.Double(nullable: false),
                        MinRequiredCount = c.Int(nullable: false),
                        MaxAddCount = c.Int(nullable: false),
                        Description = c.String(),
                        Image = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IngredientModels");
            DropTable("dbo.ConstructorCategories");
        }
    }
}
