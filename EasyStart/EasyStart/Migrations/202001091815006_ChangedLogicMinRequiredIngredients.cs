namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedLogicMinRequiredIngredients : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConstructorCategories", "MinCountIngredient", c => c.Int(nullable: false));
            DropColumn("dbo.IngredientModels", "MinRequiredCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IngredientModels", "MinRequiredCount", c => c.Int(nullable: false));
            DropColumn("dbo.ConstructorCategories", "MinCountIngredient");
        }
    }
}
