namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AreaDeliveryModels",
                c => new
                    {
                        UniqId = c.String(nullable: false, maxLength: 128),
                        DeliverySettingId = c.Int(nullable: false),
                        NameArea = c.String(),
                        MinPrice = c.Double(nullable: false),
                        DeliveryPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.UniqId);
            
            CreateTable(
                "dbo.BranchModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                        TypeBranch = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CashbackTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionType = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Money = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CategoryModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        Image = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        Visible = c.Boolean(nullable: false),
                        CategoryType = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(),
                        Password = c.String(),
                        Email = c.String(),
                        UserName = c.String(),
                        Date = c.DateTime(nullable: false),
                        ReferralCode = c.String(),
                        ParentReferralClientId = c.Int(nullable: false),
                        ParentReferralCode = c.String(),
                        VirtualMoney = c.Double(nullable: false),
                        ReferralDiscount = c.Double(nullable: false),
                        CityId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConstructorCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        MinCountIngredient = c.Int(nullable: false),
                        MaxCountIngredient = c.Int(nullable: false),
                        StyleTypeIngredient = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CouponModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        Promocode = c.String(),
                        Count = c.Int(nullable: false),
                        RewardType = c.Int(nullable: false),
                        DiscountValue = c.Int(nullable: false),
                        DiscountType = c.Int(nullable: false),
                        CountBonusProducts = c.Int(nullable: false),
                        AllowedBonusProductsJSON = c.String(),
                        CountUsed = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsOneCouponOneClient = c.Boolean(nullable: false),
                        UniqId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliverySettingModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        PayCard = c.Boolean(nullable: false),
                        PayCash = c.Boolean(nullable: false),
                        IsSoundNotify = c.Boolean(nullable: false),
                        NotificationEmail = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        ZoneId = c.String(),
                        TimeDeliveryJSON = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FCMDeviceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        Token = c.String(),
                        Platform = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IngredientModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        SubCategoryId = c.Int(nullable: false),
                        Name = c.String(),
                        AdditionalInfo = c.String(),
                        Price = c.Double(nullable: false),
                        MaxAddCount = c.Int(nullable: false),
                        Description = c.String(),
                        Image = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        DeliveryType = c.Int(nullable: false),
                        Street = c.String(),
                        HomeNumber = c.String(),
                        EntranceNumber = c.String(),
                        ApartamentNumber = c.String(),
                        Level = c.String(),
                        IntercomCode = c.String(),
                        BuyType = c.Int(nullable: false),
                        Comment = c.String(),
                        ProductCountJSON = c.String(),
                        DiscountPercent = c.Double(nullable: false),
                        DiscountRuble = c.Double(nullable: false),
                        ReferralDiscount = c.Double(nullable: false),
                        DeliveryPrice = c.Double(nullable: false),
                        CashBack = c.Double(nullable: false),
                        AmountPay = c.Double(nullable: false),
                        AmountPayDiscountDelivery = c.Double(nullable: false),
                        NeedCashBack = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        ProductBonusCountJSON = c.String(),
                        AmountPayCashBack = c.Double(nullable: false),
                        CouponId = c.Int(nullable: false),
                        ProductConstructorCountJSON = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderStockApplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        StockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PartnersTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionType = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        ReferralId = c.Int(nullable: false),
                        Money = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        Reviewer = c.String(),
                        ProductId = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        ReviewText = c.String(),
                        Date = c.DateTime(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(),
                        AdditionInfo = c.String(),
                        Description = c.String(),
                        Price = c.Double(nullable: false),
                        Image = c.String(),
                        Rating = c.Double(nullable: false),
                        VotesSum = c.Double(nullable: false),
                        VotesCount = c.Int(nullable: false),
                        ProductType = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        Visible = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PromotionCashbackSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        IsUseCashback = c.Boolean(nullable: false),
                        ReturnedValue = c.Int(nullable: false),
                        PaymentValue = c.Int(nullable: false),
                        DateSave = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PromotionPartnerSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        IsUsePartners = c.Boolean(nullable: false),
                        CashBackReferralValue = c.Int(nullable: false),
                        IsCashBackReferralOnce = c.Boolean(nullable: false),
                        TypeBonusValue = c.Int(nullable: false),
                        BonusValue = c.Int(nullable: false),
                        DateSave = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PromotionSectionSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Priorety = c.Int(nullable: false),
                        PromotionSection = c.Int(nullable: false),
                        Intersections = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PromotionSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        IsShowStockBanner = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PushMessageModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Title = c.String(),
                        Body = c.String(),
                        ImageUrl = c.String(),
                        DataJSON = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RatingProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Score = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefundCashbackTransactionModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CashbackTransactionId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
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
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StockModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        StockTypePeriod = c.Int(nullable: false),
                        StockOneTypeSubtype = c.Int(nullable: false),
                        StockFromDate = c.DateTime(nullable: false),
                        StockToDate = c.DateTime(nullable: false),
                        RewardType = c.Int(nullable: false),
                        DiscountValue = c.Int(nullable: false),
                        DiscountType = c.Int(nullable: false),
                        CountBonusProducts = c.Int(nullable: false),
                        UniqId = c.Guid(nullable: false),
                        AllowedBonusProductsJSON = c.String(),
                        ConditionType = c.Int(nullable: false),
                        ConditionDeliveryType = c.Int(nullable: false),
                        ConditionOrderSum = c.Int(nullable: false),
                        ConditionCountProductsJSON = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UseModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Use = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UseModels");
            DropTable("dbo.StockModels");
            DropTable("dbo.SettingModels");
            DropTable("dbo.RefundCashbackTransactionModels");
            DropTable("dbo.RatingProducts");
            DropTable("dbo.PushMessageModels");
            DropTable("dbo.PromotionSettings");
            DropTable("dbo.PromotionSectionSettings");
            DropTable("dbo.PromotionPartnerSettings");
            DropTable("dbo.PromotionCashbackSettings");
            DropTable("dbo.ProductModels");
            DropTable("dbo.ProductReviews");
            DropTable("dbo.PartnersTransactions");
            DropTable("dbo.OrderStockApplies");
            DropTable("dbo.OrderModels");
            DropTable("dbo.IngredientModels");
            DropTable("dbo.FCMDeviceModels");
            DropTable("dbo.DeliverySettingModels");
            DropTable("dbo.CouponModels");
            DropTable("dbo.ConstructorCategories");
            DropTable("dbo.Clients");
            DropTable("dbo.CategoryModels");
            DropTable("dbo.CashbackTransactions");
            DropTable("dbo.BranchModels");
            DropTable("dbo.AreaDeliveryModels");
        }
    }
}
