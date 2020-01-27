namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CouponAllowedProductBonus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CouponModels", "AllowedBounusProductsJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CouponModels", "AllowedBounusProductsJSON");
        }
    }
}
