namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedApproximateDeliveryTimeInOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "ApproximateDeliveryTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "ApproximateDeliveryTime");
        }
    }
}
