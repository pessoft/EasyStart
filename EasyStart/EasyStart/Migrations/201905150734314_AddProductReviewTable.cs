namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddProductReviewTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PorudctId = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        ReviewText = c.String(),
                        Reviewer = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.ProductReviews");
        }
    }
}
