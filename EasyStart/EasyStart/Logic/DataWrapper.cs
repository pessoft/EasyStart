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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return setting;
        }

        public static List<AreaDeliveryModel> GetAreaDeliveris(int delvierySettingId)
        {
            List<AreaDeliveryModel> areaDeliveries = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    areaDeliveries = db.AreaDeliveryModels
                        .Where(p => p.DeliverySettingId == delvierySettingId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return areaDeliveries;
        }

        public static DeliverySettingModel GetDeliverySetting(int branchId)
        {
            DeliverySettingModel setting = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    setting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == branchId);
                    setting.AreaDeliveries = GetAreaDeliveris(setting.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

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
                    setting.AreaDeliveries = GetAreaDeliveris(setting.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return branch == null ? TypeBranch.SubBranch : branch.TypeBranch;
        }

        public static BranchModel GetMainBranch()
        {
            BranchModel branch = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    branch = db.Branches.FirstOrDefault(p => p.TypeBranch == TypeBranch.MainBranch);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return branch;
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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return hasMainBranch;
        }

        public static BranchModel SaveBranch(BranchModel branch)
        {
            BranchModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Branches.Add(branch);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return success;
        }

        public static bool SaveAreaDeliveries(List<AreaDeliveryModel> areaDeliveries)
        {
            var success = false;

            if (areaDeliveries == null || !areaDeliveries.Any())
                return success;

            try
            {
                var dict = areaDeliveries.ToDictionary(p => p.UniqId);
                var ids = dict.Keys.ToList();

                using (var db = new AdminPanelContext())
                {
                    var allAreas = db.AreaDeliveryModels.ToList();
                    var updates = allAreas
                        .Where(x => ids.Contains(x.UniqId))
                        .ToList();
                    var newAreas = areaDeliveries
                        .Where(p => !updates.Exists(x => p.UniqId == x.UniqId))
                        .ToList();
                    var removeAreas = allAreas
                        .Where(x => !updates.Exists(p => p.UniqId == x.UniqId))
                        .ToList();

                    if (removeAreas != null && removeAreas.Any())
                    {
                        db.AreaDeliveryModels.RemoveRange(removeAreas);
                    }

                    if (updates != null)
                    {
                        updates.ForEach(p =>
                        {
                            p.NameArea = dict[p.UniqId].NameArea;
                            p.MinPrice = dict[p.UniqId].MinPrice;
                        });
                    }

                    if (areaDeliveries.Any())
                    {
                        db.AreaDeliveryModels.AddRange(newAreas);
                    }

                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
                        updateSetting.TimeDeliveryJSON = setting.TimeDeliveryJSON;
                        updateSetting.ZoneId = setting.ZoneId;
                        updateSetting.IsSoundNotify = setting.IsSoundNotify;
                        updateSetting.NotificationEmail = setting.NotificationEmail;
                    }
                    else
                    {
                        db.DeliverySettings.Add(setting);
                    }

                    db.SaveChanges();

                    var deliverySettingId = db.DeliverySettings.FirstOrDefault(p => p.BranchId == setting.BranchId);
                    setting.AreaDeliveries.ForEach(p => p.DeliverySettingId = deliverySettingId.Id);

                    var saveAreaSuccess = SaveAreaDeliveries(setting.AreaDeliveries);

                    success = true && saveAreaSuccess;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return success;
        }

        public static CategoryModel SaveCategory(CategoryModel category)
        {
            CategoryModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var orderNumber = db.Categories.Where(p => p.BranchId == category.BranchId).Count() + 1;

                    category.OrderNumber = orderNumber;
                    result = db.Categories.Add(category);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static ConstructorCategory AddOrUpdateConstructorCategory(ConstructorCategory category)
        {
            ConstructorCategory result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {

                    if (category.Id > 0)
                    {
                        var oldCategory = db.ConstructorCategories.FirstOrDefault(p => p.Id == category.Id);

                        if (oldCategory != null)
                        {
                            oldCategory.MinCountIngredient = category.MinCountIngredient;
                            oldCategory.MaxCountIngredient = category.MaxCountIngredient;
                            oldCategory.Name = category.Name;
                            oldCategory.StyleTypeIngredient = category.StyleTypeIngredient;

                            result = oldCategory;
                        }
                    }
                    else
                    {
                        var orderNumber = db.ConstructorCategories.Where(p => p.BranchId == category.BranchId).Count() + 1;

                        category.OrderNumber = orderNumber;
                        result = db.ConstructorCategories.Add(category);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static void RecalcConstructorCategoryOrderNumber(int categoryId)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var items = db.ConstructorCategories
                        .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                        .OrderBy(p => p.OrderNumber)
                        .ToList();

                    for (var i = 0; i < items.Count; ++i)
                    {
                        items[i].OrderNumber = i + 1;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static void RemoveConstructorCategory(int id)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {

                    var oldCategory = db.ConstructorCategories.FirstOrDefault(p => p.Id == id);

                    if (oldCategory != null)
                    {
                        oldCategory.IsDeleted = true;
                    }

                    db.SaveChanges();
                    RecalcConstructorCategoryOrderNumber(oldCategory.CategoryId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static List<IngredientModel> AddOrUpdateIngredients(List<IngredientModel> ingredients)
        {
            List<IngredientModel> result = null;

            if (ingredients == null || !ingredients.Any())
                return result;

            try
            {
                using (var db = new AdminPanelContext())
                {

                    var updateIngredients = ingredients.Where(p => p.Id > 0).ToDictionary(p => p.Id);
                    var newIngredients = ingredients.Where(p => p.Id < 0).ToList();
                    var subCategoryId = ingredients.First().SubCategoryId;

                    if (updateIngredients.Any())
                    {
                        var ids = updateIngredients.Keys.ToList();
                        var dataForUpdate = db.Ingredients.Where(p => ids.Contains(p.Id)).ToList();

                        if (dataForUpdate != null && dataForUpdate.Any())
                        {
                            dataForUpdate.ForEach(p =>
                            {
                                var ingredient = updateIngredients[p.Id];

                                p.Image = ingredient.Image;
                                p.IsDeleted = ingredient.IsDeleted;
                                p.MaxAddCount = ingredient.MaxAddCount;
                                p.AdditionalInfo = ingredient.AdditionalInfo;
                                p.Name = ingredient.Name;
                                p.Price = ingredient.Price;
                                p.Description = ingredient.Description;
                            });
                        }

                        db.SaveChanges();
                    }

                    if (newIngredients != null && newIngredients.Any())
                    {
                        db.Ingredients.AddRange(newIngredients);
                    }

                    db.SaveChanges();

                    result = db.Ingredients.Where(p => p.SubCategoryId == subCategoryId && !p.IsDeleted).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static void RemoveIngredientsByCategoryConstructorId(int categoryConstructorId)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var dataForRemove = db.Ingredients.Where(p => p.SubCategoryId == categoryConstructorId && !p.IsDeleted).ToList();

                    if (dataForRemove != null && dataForRemove.Any())
                    {
                        dataForRemove.ForEach(p => p.IsDeleted = true);
                        db.SaveChanges();
                    }



                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

        }

        public static CategoryModel SaveCategoryWihoutChangeOrderNumber(CategoryModel category)
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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<CategoryModel> GetCategories(int branchId)
        {
            List<CategoryModel> result = new List<CategoryModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Categories.Where(p => p.BranchId == branchId && !p.IsDeleted).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<CategoryModel> GetCategoriesVisible(int brancId)
        {
            List<CategoryModel> result = new List<CategoryModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Categories
                        .Where(p => p.BranchId == brancId && p.Visible && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static void RecalcCategoryOrderNumber(int branchId)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var categories = db.Categories
                        .Where(p => p.BranchId == branchId && !p.IsDeleted)
                        .OrderBy(p => p.OrderNumber)
                        .ToList();

                    for (var i = 0; i < categories.Count; ++i)
                    {
                        categories[i].OrderNumber = i + 1;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static bool RemoveCategory(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var removeCategory = db.Categories.FirstOrDefault(p => p.Id == id);

                    if (removeCategory != null)
                    {
                        removeCategory.IsDeleted = true;
                        db.SaveChanges();

                        RecalcCategoryOrderNumber(removeCategory.BranchId);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static void SaveProductsWihoutChangeOrderNumber(List<ProductModel> products)
        {
            ProductModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    db.Products.AddRange(products);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
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
                        .Where(p => p.CategoryId == idCategory && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<ProductModel> GetAllProducts(int branchId)
        {
            List<ProductModel> result = new List<ProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .Where(p => p.BranchId == branchId && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<int, List<ProductModel>> GetAllProductsVisibleDictionary(int branchId)
        {
            var result = new Dictionary<int, List<ProductModel>>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var products = db
                        .Products
                        .Where(p => p.BranchId == branchId && p.Visible && !p.IsDeleted)
                        .ToList();

                    if (products != null && products.Any())
                    {
                        result = products
                            .GroupBy(p => p.CategoryId)
                            .ToDictionary(p => p.Key, p => p.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<ProductModel> GetAllProductsVisible(int branchId)
        {
            List<ProductModel> result = new List<ProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Products
                        .Where(p => p.BranchId == branchId && p.Visible && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static void RecalcProductsOrderNumber(int categoryId)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var products = db.Products
                        .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                        .OrderBy(p => p.OrderNumber)
                        .ToList();

                    for (var i = 0; i < products.Count; ++i)
                    {
                        products[i].OrderNumber = i + 1;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static bool RemoveProduct(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var removeProduct = db.Products.FirstOrDefault(p => p.Id == id);

                    if (removeProduct != null)
                    {
                        removeProduct.IsDeleted = true;
                        db.SaveChanges();

                        RecalcProductsOrderNumber(removeProduct.CategoryId);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return alloweCity;
        }

        private static void SetStockIdsInOrder(OrderModel order)
        {
            if (order == null)
                return;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var stockIds = db.OrderStockApplies
                        .Where(p => p.OrderId == order.Id)
                        .Select(p => p.StockId)
                        .ToList();

                    if (stockIds != null && stockIds.Any())
                    {
                        order.StockIds = stockIds;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        private static void SetStockIdsInOrder(List<OrderModel> orders)
        {
            if (orders == null || !orders.Any())
                return;

            try
            {
                var orderIds = orders.Select(p => p.Id).ToList();
                using (var db = new AdminPanelContext())
                {
                    var stockDictIds = db.OrderStockApplies
                        .Where(p => orderIds.Contains(p.OrderId))
                        .GroupBy(p => p.OrderId)
                        .ToDictionary(p => p.Key, p => p.Select(s => s.StockId).ToList());

                    if (stockDictIds != null && stockDictIds.Any())
                    {
                        orders.ForEach(p =>
                        {
                            List<int> stockIds = null;

                            if (stockDictIds.TryGetValue(p.Id, out stockIds))
                            {
                                p.StockIds = stockIds;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
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

                    if (order.StockIds != null && order.StockIds.Any())
                    {
                        order.StockIds.ForEach(p =>
                        {
                            db.OrderStockApplies.Add(new OrderStockApply
                            {
                                OrderId = numberOrder,
                                StockId = p
                            });
                        });

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return numberOrder;
        }

        public static List<int> GetUsedOneOffStockIds(int clientId, List<int> stockIds)
        {
            List<int> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var orderIds = db.Orders
                        .Where(p => p.ClientId == clientId)
                        .Select(p => p.Id)
                        .ToList();

                    result = db.OrderStockApplies
                        .Where(p => orderIds.Contains(p.OrderId) && stockIds.Contains(p.StockId))
                        .Select(p => p.StockId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static bool IsEmptyOrders(int clientId)
        {
            var isEmpty = true;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    isEmpty = db.Orders
                        .Where(p => p.ClientId == clientId).Count() == 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return isEmpty;
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

                    if (orders != null && orders.Any())
                    {
                        SetStockIdsInOrder(orders);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return orders;
        }

        public static OrderModel GetOrder(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    order = db.Orders.FirstOrDefault(p => p.Id == orderId);
                    SetStockIdsInOrder(order);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return order;
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

                    if (orders != null && orders.Any())
                    {
                        SetStockIdsInOrder(orders);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return orders;
        }

        /// <summary>
        /// Method for mobile app API
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static List<OrderModel> GetHistoryOrders(int clientId, int branchId)
        {
            var histroyOrders = new List<OrderModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    histroyOrders = db.Orders
                        .Where(p => p.ClientId == clientId && p.BranchId == branchId)
                        .ToList();

                    if (histroyOrders != null && histroyOrders.Any())
                    {
                        SetStockIdsInOrder(histroyOrders);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return histroyOrders;
        }

        public static ProductReview SaveProductReviews(ProductReview reviews)
        {
            var result = reviews;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var saved = db.ProductReviews.Add(reviews);
                    db.SaveChanges();

                    if(saved != null)
                    {
                        result.Id = saved.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = null;
            }

            return result;
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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<int, int> GetProductReviewsVisibleCount(List<int> productIds)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.ProductReviews
                        .Where(p => productIds.Contains(p.ProductId) && p.Visible)
                        .GroupBy(p => p.ProductId)
                        .ToDictionary(p => p.Key, p => p.Count());

                    foreach (var id in productIds)
                    {
                        if (!result.ContainsKey(id))
                        {
                            result.Add(id, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static StockModel SaveStock(StockModel stock)
        {
            StockModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var isNewStock = true;

                    if (stock.Id > 0)
                    {
                        var oldStock = db.Stocks.FirstOrDefault(p => p.Id == stock.Id);

                        if (oldStock != null)
                        {
                            isNewStock = false;
                            oldStock.IsDeleted = true;
                            stock.UniqId = oldStock.UniqId;
                        }

                        db.SaveChanges();
                    }

                    if (isNewStock)
                        stock.UniqId = Guid.NewGuid();

                    result = db.Stocks.Add(stock);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<StockModel> GetActiveStocks(int branchId)
        {
            List<StockModel> result = new List<StockModel>();
            var date = DateTime.Now.Date;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks
                        .Where(p => p.BranchId == branchId && !p.IsDeleted
                        && (p.StockTypePeriod != StockTypePeriod.ToDate
                        || (DbFunctions.TruncateTime(p.StockFromDate) >= date
                        && DbFunctions.TruncateTime(p.StockToDate) >= date)))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<int> GetStockIdsByGuid(List<Guid> items)
        {
            List<int> result = new List<int>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks
                        .Where(p => items.Contains(p.UniqId))
                        .Select(p => p.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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
                        .Where(p => p.BranchId == branchId && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

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

                    removeStock.IsDeleted = true;
                    db.SaveChanges();

                    success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return success;
        }

        public static Client RegistrationClient(Client client)
        {
            Client newClient = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    newClient = db.Clients.FirstOrDefault(p => p.PhoneNumber == client.PhoneNumber);

                    if(newClient != null)
                    {
                        return null;
                    }

                    client.Date = DateTime.Now;
                    newClient = db.Clients.Add(client);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return newClient;
        }

        public static Client UpdateClient(Client client)
        {
            Client newClient = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (client.Id > 0)
                    {
                        newClient = db.Clients.FirstOrDefault(p => p.Id == client.Id);
                    }
                 

                    if (newClient != null)
                    {
                        newClient.UserName = client.UserName;
                        newClient.Email = client.Email;
                        newClient.ParentReferralClientId = client.ParentReferralClientId;
                        newClient.ParentReferralCode = client.ParentReferralCode;
                        newClient.ReferralDiscount = client.ReferralDiscount;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return newClient;
        }

        public static void ClientUpdateVirtualMoney(int clientId, double virtualMoney)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var client = db.Clients.FirstOrDefault(p => p.Id == clientId);

                    if (client != null)
                    {
                        client.VirtualMoney = virtualMoney;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static void ClientUpdateReferralDiscount(int clientId, double discount)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var client = db.Clients.FirstOrDefault(p => p.Id == clientId);

                    if (client != null)
                    {
                        client.ReferralDiscount = discount;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }
        }

        public static void UpdateOrderNumberConstructorProducts(List<UpdaterOrderNumber> upData)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var dict = upData.ToDictionary(p => p.Id, p => p.OrderNumber);
                    var ids = dict.Keys.ToList();
                    var data = db.ConstructorCategories.Where(p => ids.Contains(p.Id));

                    foreach (var up in data)
                    {
                        up.OrderNumber = dict[up.Id];
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }
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
            {
                Logger.Log.Error(ex);
            }

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
            {
                Logger.Log.Error(ex);
            }

            return image;
        }

        public static bool ValidatEmail(string phoneNumber, string email)
        {
            var result = false;

            try
            {
                using (var db = new AdminPanelContext())
                {
                   var client = db.Clients.FirstOrDefault(p => p.PhoneNumber != phoneNumber && p.Email == email);
                    result = client == null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Client GetClientByEmail(string email)
        {
            Client client = null;

            if(string.IsNullOrEmpty(email))
            {
                return client;
            }

            try
            {
                using (var db = new AdminPanelContext())
                {
                    client = db.Clients.FirstOrDefault(p => p.Email == email);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return client;
        }

        public static Client GetClient(int clientId)
        {
            Client client = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    client = db.Clients.FirstOrDefault(p => p.Id == clientId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return client;
        }

        public static Client GetClientByPhoneNumber(string phoneNumber)
        {
            Client client = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    client = db.Clients.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return client;
        }

        public static Client GetClient(string phoneNumber, string password)
        {
            Client client = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    client = db.Clients.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.Password == password);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return client;
        }

        public static Client GetClientByByReferralCode(string referralCode)
        {
            Client client = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    client = db.Clients.FirstOrDefault(p => p.ReferralCode == referralCode);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return client;
        }

        public static TodayDataOrdersModel GetDataOrdersByDate(List<int> brandchIds, DateTime date)
        {
            var todayData = new TodayDataOrdersModel();
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
                                ++todayData.CountSuccesOrder;
                                todayData.Revenue += p.AmountPayDiscountDelivery;
                            }

                            if (p.OrderStatus == OrderStatus.Cancellation)
                                ++todayData.CountCancelOrder;
                        });
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                todayData = null;
            }

            return todayData;
        }

        public static RatingProductMiddleware GetProductRating(int productId)
        {
            RatingProductMiddleware rating = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var votes = db.RatingProducts.Where(p => p.ProductId == productId);
                    var countTMP = votes.Count();
                    var sumTMP = votes.Sum(p => p.Score);
                    var ratingTMP = sumTMP / countTMP;

                    rating = new RatingProductMiddleware
                    {
                        Rating = ratingTMP,
                        VotesCount = countTMP,
                        VotesSum = sumTMP
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return rating;
        }

        public static void SaveRating(RatingProduct rating)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var findRating = db.RatingProducts.FirstOrDefault(p => p.ClientId == rating.ClientId && p.ProductId == rating.ProductId);

                    if (findRating != null)
                    {
                        findRating.Score = rating.Score;
                    }
                    else
                    {
                        db.RatingProducts.Add(rating);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static List<CouponModel> GetActiveCoupons(int branchId)
        {
            var coupons = new List<CouponModel>();
            var date = DateTime.Now.Date;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    coupons = db.Coupons
                        .Where(p => !p.IsDeleted && p.BranchId == branchId
                        && DbFunctions.TruncateTime(p.DateFrom) >= date
                        && DbFunctions.TruncateTime(p.DateFrom) <= date
                        && p.CountUsed < p.Count)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return coupons;
        }

        public static CouponModel GetCouponByPromocode(CouponParamsModel data)
        {
            CouponModel coupon = null;

            if (string.IsNullOrEmpty(data.Promocode) || data.ClientId <= 0)
                return coupon;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var date = DateTime.Now.Date;
                    coupon = db.Coupons
                        .Where(p => !p.IsDeleted
                        && p.BranchId == data.BranchId
                        && p.Promocode == data.Promocode
                        && DbFunctions.TruncateTime(p.DateFrom) <= date
                        && DbFunctions.TruncateTime(p.DateTo) >= date
                        && p.CountUsed < p.Count)
                        .FirstOrDefault();

                    if (coupon != null && coupon.IsOneCouponOneClient)
                    {
                        var coupons = db.Coupons.Where(p => p.UniqId == coupon.UniqId).Select(p => p.Id).ToList();
                        var isFirstUseCoupon = db.Orders.FirstOrDefault(p => p.ClientId == data.ClientId && coupons.Contains(p.CouponId) && p.OrderStatus != OrderStatus.Cancellation) == null;

                        if (!isFirstUseCoupon)
                            coupon = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return coupon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Идентификатор купона</param>
        /// <returns></returns>
        public static CouponModel GetCoupon(int id)
        {
            CouponModel coupon = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    coupon = db.Coupons.FirstOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return coupon;
        }

        public static List<CouponModel> GetCoupons(int branchId)
        {
            var coupons = new List<CouponModel>();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    coupons = db.Coupons
                        .Where(p => !p.IsDeleted && p.BranchId == branchId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return coupons;
        }

        public static CouponModel SaveCoupon(CouponModel newCoupon)
        {
            CouponModel result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var isNewCoupon = true;

                    if (newCoupon.Id > 0)
                    {
                        var oldCoupon = db.Coupons.FirstOrDefault(p => p.Id == newCoupon.Id);

                        if (oldCoupon != null)
                        {
                            isNewCoupon = false;
                            newCoupon.CountUsed = oldCoupon.CountUsed;
                            newCoupon.UniqId = oldCoupon.UniqId;
                            RemoveCoupon(newCoupon.Id);
                        }

                    }

                    if (isNewCoupon)
                        newCoupon.UniqId = Guid.NewGuid();

                    result = db.Coupons.Add(newCoupon);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static bool RemoveCoupon(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var oldCoupon = db.Coupons.FirstOrDefault(p => p.Id == id);

                    if (oldCoupon != null)
                    {
                        oldCoupon.IsDeleted = true;
                        db.SaveChanges();
                    }

                    success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return success;
        }

        public static void UpdateCouponCountUser(int id, int countUsed)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var coupon = db.Coupons.FirstOrDefault(p => p.Id == id);

                    if (coupon != null)
                    {
                        coupon.CountUsed = countUsed;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static PromotionCashbackSetting GetPromotionCashbackSetting(int branchId)
        {
            PromotionCashbackSetting result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.PromotionCashbackSettings.FirstOrDefault(p => p.BranchId == branchId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }


        public static PromotionPartnerSetting GetPromotionPartnerSetting(int branchId)
        {
            PromotionPartnerSetting result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.PromotionPartnerSettings.FirstOrDefault(p => p.BranchId == branchId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static PromotionCashbackSetting SavePromotionCashbackSetting(PromotionCashbackSetting setting)
        {
            PromotionCashbackSetting result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (setting.Id > 0)
                    {
                        var oldSetting = db.PromotionCashbackSettings.FirstOrDefault(p => p.Id == setting.Id);

                        if (oldSetting != null)
                        {
                            oldSetting.IsUseCashback = setting.IsUseCashback;
                            oldSetting.PaymentValue = setting.PaymentValue;
                            oldSetting.ReturnedValue = setting.ReturnedValue;
                            oldSetting.DateSave = setting.DateSave;

                            result = oldSetting;
                        }
                    }
                    else
                    {
                        result = db.PromotionCashbackSettings.Add(setting);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static PromotionPartnerSetting SavePromotionPartnerSetting(PromotionPartnerSetting setting)
        {
            PromotionPartnerSetting result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (setting.Id > 0)
                    {
                        var oldSetting = db.PromotionPartnerSettings.FirstOrDefault(p => p.Id == setting.Id);

                        if (oldSetting != null)
                        {
                            oldSetting.IsUsePartners = setting.IsUsePartners;
                            oldSetting.CashBackReferralValue = setting.CashBackReferralValue;
                            oldSetting.TypeBonusValue = setting.TypeBonusValue;
                            oldSetting.BonusValue = setting.BonusValue;
                            oldSetting.DateSave = setting.DateSave;
                            oldSetting.IsCashBackReferralOnce = setting.IsCashBackReferralOnce;

                            result = oldSetting;
                        }
                    }
                    else
                    {
                        result = db.PromotionPartnerSettings.Add(setting);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<PromotionSectionSetting> SavePromotionSectionSettings(List<PromotionSectionSetting> settings)
        {
            var result = new List<PromotionSectionSetting>();

            if (settings == null || !settings.Any())
                return result;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    foreach (var setting in settings)
                    {
                        if (setting.Id > 0)
                        {
                            var oldSetting = db.PromotionSectionSettings.FirstOrDefault(p => p.Id == setting.Id);

                            if (oldSetting != null)
                            {
                                oldSetting.Intersections = setting.Intersections;
                                oldSetting.Priorety = setting.Priorety;

                                result.Add(oldSetting);
                            }
                        }
                        else
                        {
                            var tmpResult = db.PromotionSectionSettings.Add(setting);
                            result.Add(tmpResult);
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static PromotionSetting SavePromotionSetting(PromotionSetting setting)
        {
            var result = new PromotionSetting();

            if (setting == null)
                return result;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    PromotionSetting oldSetting = null;

                    if (setting.Id > 0)
                    {
                        oldSetting = db.PromotionSettings.FirstOrDefault(p => p.Id == setting.Id);
                    }

                    if (oldSetting != null)
                    {
                        oldSetting.IsShowStockBanner = setting.IsShowStockBanner;
                        result = oldSetting;
                    }
                    else
                    {
                        result = db.PromotionSettings.Add(setting);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<PromotionSectionSetting> GetPromotionSectionSettings(int branchId)
        {
            List<PromotionSectionSetting> result = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.PromotionSectionSettings
                        .Where(p => p.BranchId == branchId)
                        .OrderBy(p => p.Priorety)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static PromotionSetting GetPromotionSetting(int branchId)
        {
            PromotionSetting result = new PromotionSetting();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var setting = db.PromotionSettings.FirstOrDefault(p => p.BranchId == branchId);

                    if (setting != null)
                        result = setting;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<ConstructorCategory> GetConstructorCategoriesVisible(int idCategory)
        {
            List<ConstructorCategory> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .ConstructorCategories
                        .Where(p => p.CategoryId == idCategory && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<ConstructorCategory> GetConstructorCategories(int idCategory)
        {
            List<ConstructorCategory> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .ConstructorCategories
                        .Where(p => p.CategoryId == idCategory)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<ConstructorCategory> GetConstuctorCategoriesByBranchId(int branchId)
        {
            List<ConstructorCategory> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .ConstructorCategories
                        .Where(p => p.BranchId == branchId && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<IngredientModel> GetIngredientsVisible(int idConstructorCategory)
        {
            List<IngredientModel> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Ingredients
                        .Where(p => p.SubCategoryId == idConstructorCategory && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<IngredientModel> GetIngredients(IEnumerable<int> ids)
        {
            List<IngredientModel> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Ingredients
                        .Where(p => ids.Contains(p.Id))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// key - subcategory id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static Dictionary<int,List<IngredientModel>> GetAllDictionaryIngredientsByCategoryIds(IEnumerable<int> ids)
        {
            Dictionary<int, List<IngredientModel>> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Ingredients
                        .Where(p => ids.Contains(p.CategoryId))
                        .GroupBy(p => p.SubCategoryId)
                        .ToDictionary(p => p.Key, p => p.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idsConstructorCategory"></param>
        /// <returns>key - IdConstructorCategory, value - List<IngredientModel></returns>
        public static Dictionary<int, List<IngredientModel>> GetIngredientsVisible(List<int> idsConstructorCategory)
        {
            Dictionary<int, List<IngredientModel>> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Ingredients
                        .Where(p => idsConstructorCategory.Contains(p.SubCategoryId) && !p.IsDeleted)
                        .GroupBy(p => p.SubCategoryId)
                        .ToDictionary(p => p.Key, p => p.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>key - IdConstructorCategory, value - List<IngredientModel></returns>
        public static Dictionary<int, List<IngredientModel>> GetIngredientsByCategoryIdVisible(IEnumerable<int> categoryId)
        {
            Dictionary<int, List<IngredientModel>> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Ingredients
                        .Where(p => categoryId.Contains(p.CategoryId) && !p.IsDeleted)
                        .GroupBy(p => p.CategoryId)
                        .ToDictionary(p => p.Key, p => p.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }
    }
}