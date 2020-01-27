namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductsBounusJSONStockModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockModels", "AllowedBounusProductsJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockModels", "AllowedBounusProductsJSON");
        }
    }
}
