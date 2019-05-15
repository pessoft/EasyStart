namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddRatingVotesCountProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "Rating", c => c.Double(nullable: false));
            AddColumn("dbo.ProductModels", "VotesCount", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProductModels", "VotesCount");
            DropColumn("dbo.ProductModels", "Rating");
        }
    }
}
