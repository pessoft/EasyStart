namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RenameRatingVotesSumProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "VotesSum", c => c.Double(nullable: false));
            DropColumn("dbo.ProductModels", "VoteSum");
        }

        public override void Down()
        {
            AddColumn("dbo.ProductModels", "VoteSum", c => c.Double(nullable: false));
            DropColumn("dbo.ProductModels", "VotesSum");
        }
    }
}
