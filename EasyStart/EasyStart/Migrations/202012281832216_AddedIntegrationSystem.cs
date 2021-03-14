namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIntegrationSystem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IntegrationSystemModals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Secret = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IntegrationSystemModals");
        }
    }
}
