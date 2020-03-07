namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateBirthClietn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "DateBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "DateBirth");
        }
    }
}
