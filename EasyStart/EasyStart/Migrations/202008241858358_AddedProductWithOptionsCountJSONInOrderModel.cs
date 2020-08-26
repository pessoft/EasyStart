namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductWithOptionsCountJSONInOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "ProductWithOptionsCountJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "ProductWithOptionsCountJSON");
        }
    }
}
