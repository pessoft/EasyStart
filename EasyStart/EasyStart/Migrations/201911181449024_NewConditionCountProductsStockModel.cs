namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewConditionCountProductsStockModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockModels", "ConditionCountProductsJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockModels", "ConditionCountProductsJSON");
        }
    }
}
