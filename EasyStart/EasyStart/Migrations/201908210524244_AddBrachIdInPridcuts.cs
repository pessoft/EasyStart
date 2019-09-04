namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBrachIdInPridcuts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryModels", "BranchId", c => c.Int(nullable: false));
            AddColumn("dbo.ProductModels", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "BranchId");
            DropColumn("dbo.CategoryModels", "BranchId");
        }
    }
}
