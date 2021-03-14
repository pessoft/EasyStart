namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVendorCodeInProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "VendorCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "VendorCode");
        }
    }
}
