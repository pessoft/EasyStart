namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "CityId", c => c.Int(nullable: false));
            AddColumn("dbo.Clients", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "BranchId");
            DropColumn("dbo.Clients", "CityId");
        }
    }
}
