namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVendorCodeInAreaDeliveryModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaDeliveryModels", "VendorCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AreaDeliveryModels", "VendorCode");
        }
    }
}
