namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTimeZoneDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliverySettingModels", "ZoneId", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.DeliverySettingModels", "ZoneId");
        }
    }
}
