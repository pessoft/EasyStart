namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStockInteractionTypeInPromotionSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromotionSettings", "StockInteractionType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromotionSettings", "StockInteractionType");
        }
    }
}
