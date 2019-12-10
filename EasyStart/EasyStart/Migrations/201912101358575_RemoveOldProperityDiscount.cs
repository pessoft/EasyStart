namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOldProperityDiscount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderModels", "Discount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "Discount", c => c.Double(nullable: false));
        }
    }
}
