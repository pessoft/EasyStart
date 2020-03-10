namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPromotionNewsModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromotionNewsModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PromotionNewsModels");
        }
    }
}
