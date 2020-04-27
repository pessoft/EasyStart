namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStockExcludedProductsJSONInStockModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockModels", "StockExcludedProductsJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockModels", "StockExcludedProductsJSON");
        }
    }
}
