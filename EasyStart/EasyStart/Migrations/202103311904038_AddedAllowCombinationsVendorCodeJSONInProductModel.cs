namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAllowCombinationsVendorCodeJSONInProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "AllowCombinationsVendorCodeJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "AllowCombinationsVendorCodeJSON");
        }
    }
}
