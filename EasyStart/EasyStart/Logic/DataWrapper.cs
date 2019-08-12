using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public static class DataWrapper
    {
        public static Dictionary<int, SettingModel> GetAllSettingDictionary()
        {
            Dictionary<int, SettingModel> settingDict = new Dictionary<int, SettingModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    settingDict = db.Settings.ToDictionary(p => p.BranchId, p => p);
                }
            }
            catch (Exception ex)
            { }

            return settingDict;
        }

        public static SettingModel GetSetting(int branchId)
        {
            SettingModel setting = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    setting = db.Settings.FirstOrDefault(p => p.BranchId == branchId);
                }
            }
            catch (Exception ex)
            { }

            return setting;
        }

        public static DeliverySettingModel GetDeliverySetting(int branchId)
        {
            DeliverySettingModel setting = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    setting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == branchId);
                }
            }
            catch (Exception ex)
            { }

            return setting;
        }

        public static int GetBranchIdByCity(int cityId)
        {
            SettingModel setting = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    setting = db.Settings.FirstOrDefault(p => p.CityId == cityId); ;
                }
            }
            catch (Exception ex)
            { }

            return setting == null ? -1 : setting.BranchId;
        }

        public static SettingModel GetSettingByCity(int cityId)
        {
            SettingModel setting = null;
            int branchId = GetBranchIdByCity(cityId);
            try
            {
                using (var db = new AdminPanelContext())
                {
                    setting = db.Settings.FirstOrDefault(p => p.BranchId == branchId);
                }
            }
            catch (Exception ex)
            { }

            return setting;
        }

        public static DeliverySettingModel GetDeliverySettingByCity(int cityId)
        {
            DeliverySettingModel setting = null;
            int branchId = GetBranchIdByCity(cityId);
            try
            {
                using (var db = new AdminPanelContext())
                {
                    setting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == branchId);
                }
            }
            catch (Exception ex)
            { }

            return setting;
        }

        public static int GetBranchId(string login)
        {
            BranchModel branch = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    branch = db.Branches.FirstOrDefault(p => p.Login == login);
                }
            }
            catch (Exception ex)
            { }

            return branch == null ? -1 : branch.Id;
        }

        public static TypeBranch GetBranchType(int id)
        {
            BranchModel branch = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    branch = db.Branches.FirstOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            { }

            return branch == null ? TypeBranch.SubBranch : branch.TypeBranch;
        }

        public static List<BranchModel> GetAllBranch()
        {
            List<BranchModel> branches = new List<BranchModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    branches = db.Branches.ToList();
                }
            }
            catch (Exception ex)
            { }

            return branches;
        }

        public static BranchModel GetBranch(string login, string password)
        {
            BranchModel branch = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    branch = db.Branches.FirstOrDefault(p => p.Login == login && p.Password == password);
                }
            }
            catch (Exception ex)
            { }

            return branch;
        }

        public static bool HasMainBranch()
        {
            var hasMainBranch = true;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var branch = db.Branches.FirstOrDefault(p => p.TypeBranch == TypeBranch.MainBranch);

                    hasMainBranch = branch != null;
                }
            }
            catch (Exception ex)
            { }

            return hasMainBranch;
        }

        public static bool SaveBranch(BranchModel branch)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    db.Branches.Add(branch);
                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static bool RemoveBranch(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var branch = db.Branches.FirstOrDefault(p => p.Id == id);
                    var setting = db.Settings.FirstOrDefault(p => p.BranchId == id);
                    var deliverySetting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == id);

                    if (branch != null)
                        db.Branches.Remove(branch);

                    if (setting != null)
                        db.Settings.Remove(setting);

                    if (deliverySetting != null)
                        db.DeliverySettings.Remove(deliverySetting);

                    db.SaveChanges();

                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static bool SaveSetting(SettingModel setting)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var updateSetting = db.Settings.FirstOrDefault(p => p.BranchId == setting.BranchId);
                    if (updateSetting != null)
                    {
                        updateSetting.BranchId = setting.BranchId;
                        updateSetting.CityId = setting.CityId;
                        updateSetting.HomeNumber = setting.HomeNumber;
                        updateSetting.Street = setting.Street;
                        updateSetting.PhoneNumber = setting.PhoneNumber;
                        updateSetting.PhoneNumberAdditional = setting.PhoneNumberAdditional;
                        updateSetting.Email = setting.Email;
                        updateSetting.Vkontakte = setting.Vkontakte;
                        updateSetting.Instagram = setting.Instagram;
                        updateSetting.Facebook = setting.Facebook;
                    }
                    else
                    {
                        db.Settings.Add(setting);
                    }

                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static bool SaveDeliverySetting(DeliverySettingModel setting)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var updateSetting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == setting.BranchId);

                    if (updateSetting != null)
                    {
                        updateSetting.PayCard = setting.PayCard;
                        updateSetting.PayCash = setting.PayCash;
                        updateSetting.PriceDelivery = setting.PriceDelivery;
                        updateSetting.FreePriceDelivery = setting.FreePriceDelivery;
                        updateSetting.TimeDeliveryJSON = setting.TimeDeliveryJSON;
                        updateSetting.ZoneId = setting.ZoneId;
                        updateSetting.IsSoundNotify = setting.IsSoundNotify;
                    }
                    else
                    {
                        db.DeliverySettings.Add(setting);
                    }

                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static CategoryModel SaveCategory(CategoryModel category)
        {
            CategoryModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var orderNumber = db.Categories.Count() + 1;

                    category.OrderNumber = orderNumber;
                    result = db.Categories.Add(category);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static Dictionary<int, CategoryModel> GetCategories(IEnumerable<int> ids)
        {
            Dictionary<int, CategoryModel> result = new Dictionary<int, CategoryModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Categories
                        .Where(p => ids.Contains(p.Id))
                        .ToDictionary(p => p.Id, p => p);
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<CategoryModel> GetCategories()
        {
            List<CategoryModel> result = new List<CategoryModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Categories.ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<CategoryModel> GetCategoriesVisible()
        {
            List<CategoryModel> result = new List<CategoryModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Categories
                        .Where(p => p.Visible)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static CategoryModel UpdateCategory(CategoryModel category)
        {
            CategoryModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Categories.FirstOrDefault(p => p.Id == category.Id);
                    result.Image = category.Image;
                    result.Name = category.Name;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static bool RemoveCategory(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var removeCategory = db.Categories.FirstOrDefault(p => p.Id == id);
                    db.Categories.Remove(removeCategory);
                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static ProductModel SaveProduct(ProductModel product)
        {
            ProductModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var orderNumber = 1 + db.Products
                        .Where(p => p.CategoryId == product.CategoryId)
                        .Count();

                    product.OrderNumber = orderNumber;
                    result = db.Products.Add(product);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<ProductModel> GetProducts(int idCategory)
        {
            List<ProductModel> result = new List<ProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .Where(p => p.CategoryId == idCategory)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<ProductModel> GetOrderProducts(List<int> ids)
        {
            List<ProductModel> result = new List<ProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .Where(p => ids.Contains(p.Id))
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static void UpdateStatusOrder(UpdaterOrderStatus data)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var order = db
                        .Orders
                        .FirstOrDefault(p => data.OrderId == p.Id);

                    if (order != null)
                    {
                        order.OrderStatus = data.Status;
                        order.UpdateDate = data.DateUpdate;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }
        }

        public static void UpdateRating(int productId, double rating, int votesCount, double votesSum)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var product = db
                        .Products
                        .FirstOrDefault(p => p.Id == productId);

                    if (product != null)
                    {
                        product.Rating = rating;
                        product.VotesCount = votesCount;
                        product.VotesSum = votesSum;

                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            { }
        }

        public static ProductModel GetProduct(int productId)
        {
            ProductModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .FirstOrDefault(p => p.Id == productId);

                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<ProductModel> GetProducts(IEnumerable<int> productIds)
        {
            List<ProductModel> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToList(); ;
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<ProductModel> GetAllProducts()
        {
            List<ProductModel> result = new List<ProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<ProductModel> GetAllProductsVisible()
        {
            List<ProductModel> result = new List<ProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products.
                        Where(p => p.Visible)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static ProductModel UpdateProduct(ProductModel product)
        {
            ProductModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Products.FirstOrDefault(p => p.Id == product.Id);

                    result.Image = product.Image;
                    result.Name = product.Name;
                    result.Description = product.Description;
                    result.Price = product.Price;
                    result.AdditionInfo = product.AdditionInfo;
                    result.ProductType = product.ProductType;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static bool RemoveProduct(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var removeProduct = db.Products.FirstOrDefault(p => p.Id == id);

                    db.Products.Remove(removeProduct);
                    db.SaveChanges();

                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static List<int> GetAllowedCity()
        {
            var alloweCity = new List<int>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var allowedBranches = db.DeliverySettings
                        .Select(p => p.BranchId)
                        .Distinct()
                        .ToList();

                    alloweCity = db
                        .Settings
                        .ToList()
                        .Where(p => allowedBranches.IndexOf(p.BranchId) != -1)
                        .Select(p => p.CityId)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return alloweCity;
        }

        public static int SaveOrder(OrderModel order)
        {
            var numberOrder = -1;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var saveOrder = db.Orders.Add(order);
                    db.SaveChanges();
                    numberOrder = saveOrder.Id;
                }
            }
            catch (Exception ex)
            { }

            return numberOrder;
        }

        public static List<OrderModel> GetOrders(List<int> brandchIds)
        {
            var orders = new List<OrderModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    orders = db.Orders
                        .Where(p => brandchIds.Contains(p.BranchId) &&
                                    p.OrderStatus == OrderStatus.Processing)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return orders;
        }

        public static List<OrderModel> GetHistoryOrders(List<int> brandchIds, DateTime startDate, DateTime endDate)
        {
            var orders = new List<OrderModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    orders = db.Orders
                        .Where(p => brandchIds.Contains(p.BranchId) &&
                                    p.OrderStatus != OrderStatus.Processing &&
                                    DbFunctions.TruncateTime(p.UpdateDate) >= startDate.Date &&
                                    DbFunctions.TruncateTime(p.UpdateDate) <= endDate.Date)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return orders;
        }

        /// <summary>
        /// Method for mobile app API
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static List<OrderModel> GetHistoryOrder(int clientId)
        {
            var histroyOrders = new List<OrderModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    histroyOrders = db.Orders
                        .Where(p => p.ClientId == clientId)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return histroyOrders;
        }

        public static bool SaveProductReviews(ProductReview previews)
        {
            bool success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    db.ProductReviews.Add(previews);
                    db.SaveChanges();

                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static List<ProductReview> GetProductReviews(int productId)
        {
            List<ProductReview> result = new List<ProductReview>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.ProductReviews
                        .Where(p => p.ProductId == productId)
                        .OrderByDescending(p => p.Date)
                        .Take(50)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<ProductReview> GetProductReviewsVisible(int productId)
        {
            List<ProductReview> result = new List<ProductReview>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.ProductReviews
                        .Where(p => p.ProductId == productId && p.Visible)
                        .OrderByDescending(p => p.Date)
                        .Take(50)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }
        public static StockModel SaveStock(StockModel stock)
        {
            StockModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks.Add(stock);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<StockModel> GetStocks(int branchId)
        {
            List<StockModel> result = new List<StockModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks
                        .Where(p => p.BranchId == branchId)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<StockModel> GetStocksVisible(int brnachId)
        {
            List<StockModel> result = new List<StockModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks
                        .Where(p => p.BranchId == brnachId && p.Visible)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static List<StockModel> GetStocksVisible()
        {
            List<StockModel> result = new List<StockModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks
                        .Where(p => p.Visible)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static bool RemoveStock(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var removeStock = db.Stocks.FirstOrDefault(p => p.Id == id);

                    db.Stocks.Remove(removeStock);
                    db.SaveChanges();

                    success = true;
                }
            }
            catch (Exception ex)
            { }

            return success;
        }

        public static StockModel UpdateStock(StockModel stock)
        {
            StockModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks.FirstOrDefault(p => p.Id == stock.Id);

                    if (result != null)
                    {
                        result.Description = stock.Description;
                        result.Discount = stock.Discount;
                        result.Image = stock.Image;
                        result.Name = stock.Name;
                        result.StockType = stock.StockType;
                        result.Visible = stock.Visible;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

        public static Client AddOrUpdateClient(Client clinet)
        {
            Client newClient = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (clinet.Id != 0)
                    {
                        newClient = db.Clients.FirstOrDefault(p => p.Id == clinet.Id);
                    }

                    if (newClient != null)
                    {
                        newClient.PhoneNumber = clinet.PhoneNumber;
                    }
                    else
                    {
                        clinet.Date = DateTime.Now.Date;
                        newClient = db.Clients.Add(clinet);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return clinet;
        }

        public static void UpdateOrderNumberCategory(List<UpdaterOrderNumber> upData)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var dict = upData.ToDictionary(p => p.Id, p => p.OrderNumber);
                    var ids = dict.Keys.ToList(); ;
                    var data = db.Categories.Where(p => ids.Contains(p.Id));

                    foreach (var up in data)
                    {
                        up.OrderNumber = dict[up.Id];
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }
        }

        //to do вынести в общий метод с категориями
        public static void UpdateOrderNumberProducts(List<UpdaterOrderNumber> upData)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var dict = upData.ToDictionary(p => p.Id, p => p.OrderNumber);
                    var ids = dict.Keys.ToList();
                    var data = db.Products.Where(p => ids.Contains(p.Id));

                    foreach (var up in data)
                    {
                        up.OrderNumber = dict[up.Id];
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }
        }

        public static void UpdateVisibleCategory(UpdaterVisible upData)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var data = db.Categories.FirstOrDefault(p => p.Id == upData.Id);

                    if (data != null)
                    {
                        data.Visible = upData.Visible;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public static void UpdateVisibleProduct(UpdaterVisible upData)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var data = db.Products.FirstOrDefault(p => p.Id == upData.Id);

                    if (data != null)
                    {
                        data.Visible = upData.Visible;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public static void UpdateVisibleReview(UpdaterVisible upData)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var data = db.ProductReviews.FirstOrDefault(p => p.Id == upData.Id);

                    if (data != null)
                    {
                        data.Visible = upData.Visible;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public static string GetCategoryImage(int id)
        {
            string image = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var result = db.Categories.FirstOrDefault(p => p.Id == id);

                    image = result?.Image;

                }
            }
            catch (Exception ex)
            { }

            return image;
        }

        public static string GetProductImage(int id)
        {
            string image = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var result = db.Products.FirstOrDefault(p => p.Id == id);

                    image = result?.Image;

                }
            }
            catch (Exception ex)
            { }

            return image;
        }

        public static Client GetClient(int clinetId)
        {
            Client client = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    client = db.Clients.FirstOrDefault(p => p.Id == clinetId);
                }
            }
            catch (Exception ex)
            { }

            return client;
        }


        public static TodayDataOrdersModel GetDataOrdersByDate(List<int> brandchIds, DateTime date)
        {
            var todatData = new TodayDataOrdersModel();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    db.Orders
                        .Where(p => brandchIds.Contains(p.BranchId) &&
                                    p.OrderStatus != OrderStatus.Processing &&
                                    DbFunctions.TruncateTime(p.UpdateDate) == date.Date)
                        .ToList()
                        .ForEach(p =>
                        {
                            if (p.OrderStatus == OrderStatus.Processed)
                            {
                                ++todatData.CountSuccesOrder;
                                todatData.Revenue += p.AmountPayDiscountDelivery;
                            }

                            if (p.OrderStatus == OrderStatus.Cancellation)
                                ++todatData.CountCancelOrder;
                        });
                }
            }
            catch (Exception ex)
            {
                todatData = null;
            }

            return todatData;
        }
    }
}