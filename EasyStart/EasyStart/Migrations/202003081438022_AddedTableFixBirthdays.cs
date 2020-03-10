namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableFixBirthdays : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FixBirthdays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateUse = c.DateTime(nullable: false),
                        DateBirth = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FixBirthdays");
        }
    }
}
