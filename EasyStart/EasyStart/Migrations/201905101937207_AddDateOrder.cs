namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "Date");
        }
    }
}
