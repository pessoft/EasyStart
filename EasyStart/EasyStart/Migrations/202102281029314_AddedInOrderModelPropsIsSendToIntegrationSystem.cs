namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInOrderModelPropsIsSendToIntegrationSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "IsSendToIntegrationSystem", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "IsSendToIntegrationSystem");
        }
    }
}
