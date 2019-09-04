namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaDelivery : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AreaDeliveryModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameArea = c.String(),
                        MinPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.DeliverySettingModels", "FreePriceDelivery");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeliverySettingModels", "FreePriceDelivery", c => c.Double(nullable: false));
            DropTable("dbo.AreaDeliveryModels");
        }
    }
}
