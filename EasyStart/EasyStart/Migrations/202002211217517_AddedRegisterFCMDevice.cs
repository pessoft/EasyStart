namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRegisterFCMDevice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FCMDeviceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        Token = c.String(),
                        Platform = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FCMDeviceModels");
        }
    }
}
