namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewStockModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockModels", "StockTypePeriod", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "StockOneTypeSubtype", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "StockFromDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.StockModels", "StockToDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.StockModels", "RewardType", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "DiscountValue", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "DiscountType", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "CountBounusProducts", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "ConditionType", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "ConditionDeliveryType", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "ConditionOrderSum", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "IsDeleted", c => c.Boolean(nullable: false));
            DropColumn("dbo.StockModels", "StockType");
            DropColumn("dbo.StockModels", "Discount");
            DropColumn("dbo.StockModels", "Visible");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockModels", "Visible", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockModels", "Discount", c => c.Double(nullable: false));
            AddColumn("dbo.StockModels", "StockType", c => c.Int(nullable: false));
            DropColumn("dbo.StockModels", "IsDeleted");
            DropColumn("dbo.StockModels", "ConditionOrderSum");
            DropColumn("dbo.StockModels", "ConditionDeliveryType");
            DropColumn("dbo.StockModels", "ConditionType");
            DropColumn("dbo.StockModels", "CountBounusProducts");
            DropColumn("dbo.StockModels", "DiscountType");
            DropColumn("dbo.StockModels", "DiscountValue");
            DropColumn("dbo.StockModels", "RewardType");
            DropColumn("dbo.StockModels", "StockToDate");
            DropColumn("dbo.StockModels", "StockFromDate");
            DropColumn("dbo.StockModels", "StockOneTypeSubtype");
            DropColumn("dbo.StockModels", "StockTypePeriod");
        }
    }
}
