namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRatingVotesSumProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "VoteSum", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "VoteSum");
        }
    }
}
