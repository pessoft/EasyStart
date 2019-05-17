namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addStokTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StockType = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Discount = c.Double(nullable: false),
                        Image = c.String(),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.StockModels");
        }
    }
}
