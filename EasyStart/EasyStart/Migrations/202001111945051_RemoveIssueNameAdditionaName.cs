namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIssueNameAdditionaName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.IngredientModels", "AdditionaInfo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IngredientModels", "AdditionaInfo", c => c.String());
        }
    }
}
