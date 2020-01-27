namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixWorldReferrel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "ParentReferralClientId", c => c.Int(nullable: false));
            AddColumn("dbo.Clients", "ReferralDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderModels", "ReferralDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.PromotionPartnerSettings", "CashBackReferralValue", c => c.Int(nullable: false));
            DropColumn("dbo.Clients", "ParentRefferalClientId");
            DropColumn("dbo.Clients", "RefferalDiscount");
            DropColumn("dbo.OrderModels", "RefferalDiscount");
            DropColumn("dbo.PromotionPartnerSettings", "CashBackRefferalValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PromotionPartnerSettings", "CashBackRefferalValue", c => c.Int(nullable: false));
            AddColumn("dbo.OrderModels", "RefferalDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.Clients", "RefferalDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.Clients", "ParentRefferalClientId", c => c.Int(nullable: false));
            DropColumn("dbo.PromotionPartnerSettings", "CashBackReferralValue");
            DropColumn("dbo.OrderModels", "ReferralDiscount");
            DropColumn("dbo.Clients", "ReferralDiscount");
            DropColumn("dbo.Clients", "ParentReferralClientId");
        }
    }
}
