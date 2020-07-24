namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBranchIdInAdditionOptionItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdditionOptionItems", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdditionOptionItems", "BranchId");
        }
    }
}
