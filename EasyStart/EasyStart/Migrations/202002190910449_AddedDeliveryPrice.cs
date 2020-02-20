namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDeliveryPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaDeliveryModels", "DeliveryPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AreaDeliveryModels", "DeliveryPrice");
        }
    }
}
