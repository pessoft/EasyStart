namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedClientIdInFixBirthdays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FixBirthdays", "ClientId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FixBirthdays", "ClientId");
        }
    }
}
