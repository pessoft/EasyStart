namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNumberAppliancesInCategoryModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "NumberAppliances", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryModels", "NumberAppliances");
        }
    }
}
