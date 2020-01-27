namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "CategoryType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryModels", "CategoryType");
        }
    }
}
