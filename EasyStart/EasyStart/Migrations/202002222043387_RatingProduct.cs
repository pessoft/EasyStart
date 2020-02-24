namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RatingProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RatingProducts",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ClientId = c.Int(nullable: false),
                    ProductId = c.Int(nullable: false),
                    Score = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.OrderStockApplies");
        }
    }
}
