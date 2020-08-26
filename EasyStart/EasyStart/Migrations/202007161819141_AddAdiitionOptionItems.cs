namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdiitionOptionItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdditionOptionItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdditionOptionId = c.Int(nullable: false),
                        Name = c.String(),
                        AdditionalInfo = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AdditionOptionItems");
        }
    }
}
