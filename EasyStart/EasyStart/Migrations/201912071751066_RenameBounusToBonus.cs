namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameBounusToBonus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CouponModels", "CountBonusProducts", c => c.Int(nullable: false));
            AddColumn("dbo.CouponModels", "AllowedBonusProductsJSON", c => c.String());
            AddColumn("dbo.StockModels", "CountBonusProducts", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "AllowedBonusProductsJSON", c => c.String());
            DropColumn("dbo.CouponModels", "CountBounusProducts");
            DropColumn("dbo.CouponModels", "AllowedBounusProductsJSON");
            DropColumn("dbo.StockModels", "CountBounusProducts");
            DropColumn("dbo.StockModels", "AllowedBounusProductsJSON");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockModels", "AllowedBounusProductsJSON", c => c.String());
            AddColumn("dbo.StockModels", "CountBounusProducts", c => c.Int(nullable: false));
            AddColumn("dbo.CouponModels", "AllowedBounusProductsJSON", c => c.String());
            AddColumn("dbo.CouponModels", "CountBounusProducts", c => c.Int(nullable: false));
            DropColumn("dbo.StockModels", "AllowedBonusProductsJSON");
            DropColumn("dbo.StockModels", "CountBonusProducts");
            DropColumn("dbo.CouponModels", "AllowedBonusProductsJSON");
            DropColumn("dbo.CouponModels", "CountBonusProducts");
        }
    }
}
