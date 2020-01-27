namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCosntructorCategoryOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "ProductConstructorCountJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "ProductConstructorCountJSON");
        }
    }
}
