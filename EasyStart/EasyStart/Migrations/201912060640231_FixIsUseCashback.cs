namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixIsUseCashback : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromotionCashbackSettings", "IsUseCashback", c => c.Boolean(nullable: false));
            DropColumn("dbo.PromotionCashbackSettings", "IsUseCaschback");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PromotionCashbackSettings", "IsUseCaschback", c => c.Boolean(nullable: false));
            DropColumn("dbo.PromotionCashbackSettings", "IsUseCashback");
        }
    }
}
