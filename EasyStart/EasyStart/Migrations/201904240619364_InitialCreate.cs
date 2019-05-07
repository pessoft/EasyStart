namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BranchModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                        TypeBranch = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CategoryModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliverySettingModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        PriceDelivery = c.Double(nullable: false),
                        FreePriceDelivery = c.Double(nullable: false),
                        PayCard = c.Boolean(nullable: false),
                        PayCash = c.Boolean(nullable: false),
                        TimeDeliveryJSON = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(),
                        AdditionInfo = c.String(),
                        Description = c.String(),
                        Price = c.Double(nullable: false),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SettingModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        Street = c.String(),
                        HomeNumber = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        PhoneNumberAdditional = c.String(),
                        Email = c.String(),
                        Vkontakte = c.String(),
                        Instagram = c.String(),
                        Facebook = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SettingModels");
            DropTable("dbo.ProductModels");
            DropTable("dbo.DeliverySettingModels");
            DropTable("dbo.CategoryModels");
            DropTable("dbo.BranchModels");
        }
    }
}
