namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SplitDiscount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "DiscountPercent", c => c.Double(nullable: false));
            AddColumn("dbo.OrderModels", "DiscountRuble", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "DiscountRuble");
            DropColumn("dbo.OrderModels", "DiscountPercent");
        }
    }
}
