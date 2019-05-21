using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class AdminPanelContext: DbContext
    {
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BranchModel> Branches { get; set; }
        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<DeliverySettingModel> DeliverySettings { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<StockModel> Stocks { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}