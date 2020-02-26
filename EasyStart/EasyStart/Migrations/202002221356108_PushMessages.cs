namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PushMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PushMessageModels",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BranchId = c.Int(nullable: false),
                    Title = c.String(),
                    Body = c.String(),
                    ImageUrl = c.String(),
                    DataJSON = c.String(),
                    Date = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.PushMessageModels");
        }
    }
}
