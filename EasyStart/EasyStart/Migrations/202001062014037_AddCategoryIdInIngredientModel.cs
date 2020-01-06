namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryIdInIngredientModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IngredientModels", "CategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IngredientModels", "CategoryId");
        }
    }
}
