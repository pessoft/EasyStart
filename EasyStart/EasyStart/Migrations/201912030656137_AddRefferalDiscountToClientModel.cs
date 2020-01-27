namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefferalDiscountToClientModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "RefferalDiscount", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "RefferalDiscount");
        }
    }
}
