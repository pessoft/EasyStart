namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductAdditionalOptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductAdditionalOptionModals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        AdditionalOptionId = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductAdditionalOptionModals");
        }
    }
}
