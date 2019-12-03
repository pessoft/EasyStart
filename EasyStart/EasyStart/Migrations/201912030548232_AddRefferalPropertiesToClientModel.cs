namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefferalPropertiesToClientModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "ReferralCode", c => c.String());
            AddColumn("dbo.Clients", "PurentRefferalClientId", c => c.Int(nullable: false));
            AddColumn("dbo.Clients", "VirtualMoney", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "VirtualMoney");
            DropColumn("dbo.Clients", "PurentRefferalClientId");
            DropColumn("dbo.Clients", "ReferralCode");
        }
    }
}
