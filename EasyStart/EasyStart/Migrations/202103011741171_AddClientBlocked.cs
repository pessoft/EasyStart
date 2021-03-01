namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientBlocked : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Blocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Blocked");
        }
    }
}
