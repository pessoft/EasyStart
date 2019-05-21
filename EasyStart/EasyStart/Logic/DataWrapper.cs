using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class DataWrapper
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

                    db.Branches.Remove(branch);
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
                        updateSetting.Facebook= setting.Facebook;
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
                    result = db.Categories.Add(category);
                    db.SaveChanges();
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
                        .Where(p => p.PorudctId == productId)
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

        public static List<StockModel> GetStocks()
        {
            List<StockModel> result = new List<StockModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks.ToList();
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
                        newClient = db.Clients.Add(clinet);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }

            return clinet;
        }

    }
}