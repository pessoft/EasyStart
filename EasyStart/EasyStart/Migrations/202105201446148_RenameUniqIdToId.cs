namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameUniqIdToId : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.AreaDeliveryModels","UniqId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.AreaDeliveryModels", "Id", "UniqId");
        }
    }
}
