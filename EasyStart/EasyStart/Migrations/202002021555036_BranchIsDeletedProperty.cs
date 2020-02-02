namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BranchIsDeletedProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BranchModels", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BranchModels", "IsDeleted");
        }
    }
}
