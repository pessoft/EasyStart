namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAreaDeliveryIdInProductModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "AreaDeliveryId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "AreaDeliveryId");
        }
    }
}
