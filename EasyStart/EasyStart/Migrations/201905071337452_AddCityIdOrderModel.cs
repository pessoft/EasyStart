namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCityIdOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "CityId", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.OrderModels", "CityId");
        }
    }
}
