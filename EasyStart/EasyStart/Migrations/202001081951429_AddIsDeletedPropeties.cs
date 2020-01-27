namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsDeletedPropeties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductModels", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "IsDeleted");
            DropColumn("dbo.CategoryModels", "IsDeleted");
        }
    }
}
