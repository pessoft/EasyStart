namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlwaysApplyCashback : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromotionCashbackSettings", "AlwaysApplyCashback", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromotionCashbackSettings", "AlwaysApplyCashback");
        }
    }
}
