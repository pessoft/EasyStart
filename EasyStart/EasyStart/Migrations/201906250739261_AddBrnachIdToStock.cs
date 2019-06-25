namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBrnachIdToStock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockModels", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockModels", "BranchId");
        }
    }
}
