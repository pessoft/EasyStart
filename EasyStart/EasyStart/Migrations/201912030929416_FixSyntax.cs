namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSyntax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "ParentRefferalClientId", c => c.Int(nullable: false));
            AddColumn("dbo.PromotionPartnerSettings", "CashBackRefferalValue", c => c.Int(nullable: false));
            DropColumn("dbo.Clients", "PurentRefferalClientId");
            DropColumn("dbo.PromotionPartnerSettings", "CashBackReferalValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PromotionPartnerSettings", "CashBackReferalValue", c => c.Int(nullable: false));
            AddColumn("dbo.Clients", "PurentRefferalClientId", c => c.Int(nullable: false));
            DropColumn("dbo.PromotionPartnerSettings", "CashBackRefferalValue");
            DropColumn("dbo.Clients", "ParentRefferalClientId");
        }
    }
}
