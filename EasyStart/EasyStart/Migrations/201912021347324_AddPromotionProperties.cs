namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPromotionProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "ProductBonusCountJSON", c => c.String());
            AddColumn("dbo.OrderModels", "AmountPayCashBack", c => c.Double(nullable: false));
            AddColumn("dbo.OrderModels", "StockId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderModels", "CouponId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderModels", "IsGetCashback", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "IsPartnerBonus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "IsPartnerBonus");
            DropColumn("dbo.OrderModels", "IsGetCashback");
            DropColumn("dbo.OrderModels", "CouponId");
            DropColumn("dbo.OrderModels", "StockId");
            DropColumn("dbo.OrderModels", "AmountPayCashBack");
            DropColumn("dbo.OrderModels", "ProductBonusCountJSON");
        }
    }
}
