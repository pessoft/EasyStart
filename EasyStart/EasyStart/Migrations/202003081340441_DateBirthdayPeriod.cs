namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateBirthdayPeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockModels", "ConditionBirthdayBefore", c => c.Int(nullable: false));
            AddColumn("dbo.StockModels", "ConditionBirthdayAfter", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockModels", "ConditionBirthdayAfter");
            DropColumn("dbo.StockModels", "ConditionBirthdayBefore");
        }
    }
}
