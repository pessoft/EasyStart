namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDurationCashbeckParentReferral : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromotionPartnerSettings", "IsCashBackReferralOnce", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromotionPartnerSettings", "IsCashBackReferralOnce");
        }
    }
}
