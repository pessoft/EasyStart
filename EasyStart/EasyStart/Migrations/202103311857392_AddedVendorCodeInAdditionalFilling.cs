namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVendorCodeInAdditionalFilling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdditionalFillings", "VendorCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdditionalFillings", "VendorCode");
        }
    }
}
