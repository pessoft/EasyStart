namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAllowCombinationsJSONInProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "AllowCombinationsJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "AllowCombinationsJSON");
        }
    }
}
