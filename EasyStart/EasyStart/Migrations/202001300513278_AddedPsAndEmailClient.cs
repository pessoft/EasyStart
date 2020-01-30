namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPsAndEmailClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Password", c => c.String());
            AddColumn("dbo.Clients", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Email");
            DropColumn("dbo.Clients", "Password");
        }
    }
}
