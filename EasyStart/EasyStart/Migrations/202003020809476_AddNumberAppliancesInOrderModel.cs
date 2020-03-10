namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNumberAppliancesInOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "NumberAppliances", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "NumberAppliances");
        }
    }
}
