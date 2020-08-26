namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductAdditionalInfoTypeInProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "ProductAdditionalInfoType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "ProductAdditionalInfoType");
        }
    }
}
