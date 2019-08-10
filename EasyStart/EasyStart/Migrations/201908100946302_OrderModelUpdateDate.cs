namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderModelUpdateDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "UpdateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "UpdateDate");
        }
    }
}
