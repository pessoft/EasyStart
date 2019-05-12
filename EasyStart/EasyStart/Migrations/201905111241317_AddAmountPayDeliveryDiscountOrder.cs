namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAmountPayDeliveryDiscountOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "AmountPayDiscountDelivery", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "AmountPayDiscountDelivery");
        }
    }
}
