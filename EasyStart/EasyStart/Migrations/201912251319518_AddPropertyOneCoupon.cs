namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertyOneCoupon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CouponModels", "IsOneCouponOneClient", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CouponModels", "IsOneCouponOneClient");
        }
    }
}
