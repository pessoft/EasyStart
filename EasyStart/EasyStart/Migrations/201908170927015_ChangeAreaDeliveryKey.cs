namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAreaDeliveryKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AreaDeliveryModels");
            AddColumn("dbo.AreaDeliveryModels", "UniqId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.AreaDeliveryModels", "UniqId");
            DropColumn("dbo.AreaDeliveryModels", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AreaDeliveryModels", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.AreaDeliveryModels");
            DropColumn("dbo.AreaDeliveryModels", "UniqId");
            AddPrimaryKey("dbo.AreaDeliveryModels", "Id");
        }
    }
}
