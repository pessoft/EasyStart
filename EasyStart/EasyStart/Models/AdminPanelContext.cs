using EasyStart.Models.FCMNotification;
using EasyStart.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class AdminPanelContext : DbContext
    {
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BranchModel> Branches { get; set; }
        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<DeliverySettingModel> DeliverySettings { get; set; }
        public DbSet<AreaDeliveryModel> AreaDeliveryModels { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<StockModel> Stocks { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<UseModel> Uses { get; set; }
        public DbSet<RatingProduct> RatingProducts { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<PromotionCashbackSetting> PromotionCashbackSettings { get; set; }
        public DbSet<PromotionPartnerSetting> PromotionPartnerSettings { get; set; }
        public DbSet<PromotionSectionSetting> PromotionSectionSettings { get; set; }
        public DbSet<OrderStockApply> OrderStockApplies { get; set; }

        public DbSet<CashbackTransaction> CashbackTransactions { get; set; }
        public DbSet<RefundCashbackTransactionModel> RefundCashbackTransactions { get; set; }
        public DbSet<PartnersTransaction> PartnersTransactions { get; set; }
        public DbSet<PromotionSetting> PromotionSettings { get; set; }
        public DbSet<ConstructorCategory> ConstructorCategories { get; set; }
        public DbSet<IngredientModel> Ingredients { get; set; }

        public DbSet<PushMessageModel> PushMessages { get; set; }
    }
}