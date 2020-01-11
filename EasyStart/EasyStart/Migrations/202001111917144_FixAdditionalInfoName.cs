namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAdditionalInfoName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IngredientModels", "AdditionalInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IngredientModels", "AdditionalInfo");
        }
    }
}
