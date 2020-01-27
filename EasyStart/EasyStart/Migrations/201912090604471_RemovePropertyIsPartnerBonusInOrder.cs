namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePropertyIsPartnerBonusInOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderModels", "IsPartnerBonus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "IsPartnerBonus", c => c.Boolean(nullable: false));
        }
    }
}
