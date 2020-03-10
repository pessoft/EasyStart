namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateDeliveryInOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "DateDelivery", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "DateDelivery");
        }
    }
}
