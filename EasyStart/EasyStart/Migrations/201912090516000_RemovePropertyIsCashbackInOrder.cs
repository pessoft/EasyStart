namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePropertyIsCashbackInOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderModels", "IsGetCashback");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "IsGetCashback", c => c.Boolean(nullable: false));
        }
    }
}
