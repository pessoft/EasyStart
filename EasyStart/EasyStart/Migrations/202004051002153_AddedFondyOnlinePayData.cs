namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFondyOnlinePayData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "MerchantId", c => c.Int(nullable: false));
            AddColumn("dbo.DeliverySettingModels", "PaymentKey", c => c.String());
            AddColumn("dbo.DeliverySettingModels", "CreditKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "CreditKey");
            DropColumn("dbo.DeliverySettingModels", "PaymentKey");
            DropColumn("dbo.DeliverySettingModels", "MerchantId");
        }
    }
}
