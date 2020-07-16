namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBranchIdInAdditionalOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdditionalOptions", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdditionalOptions", "BranchId");
        }
    }
}
