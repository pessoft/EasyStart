using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
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
                    settingDict = db.Settings.Where(p => !p.IsDeleted).ToDictionary(p => p.BranchId, p => p);
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
                    setting = db.Settings.FirstOrDefault(p => p.BranchId == branchId && !p.IsDeleted);
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
                    setting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == branchId && !p.IsDeleted);

                    if (setting != null)
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
                    setting = db.Settings.FirstOrDefault(p => p.CityId == cityId && !p.IsDeleted); ;
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
                    branch = db.Branches.FirstOrDefault(p => p.Login == login && !p.IsDeleted);
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
                    var clietns = db.Clients.Where(p => p.BranchId == id).ToList();
                    var categories = db.Categories.Where(p => p.BranchId == id).ToList();
                    var products = db.Products.Where(p => p.BranchId == id).ToList();
                    var additionalOptions = db.AdditionalOptions.Where(p => p.BranchId == id && !p.IsDeleted).ToList();
                    var aditionOptionItems = db.AdditionOptionItems.Where(p => p.BranchId == id && !p.IsDeleted).ToList();
                    var productAdditionalOptions = db.ProductAdditionalOptions.Where(p => p.BranchId == id && !p.IsDeleted).ToList();
                    var additionalFillgs = db.AdditionalFillings.Where(p => p.BranchId == id && !p.IsDeleted).ToList();
                    var productAdditionalFillings = db.ProductAdditionalFillings.Where(p => p.BranchId == id && !p.IsDeleted).ToList();
                    var constructoCategories = db.ConstructorCategories.Where(p => p.BranchId == id).ToList();
                    var categoryIds = categories.Select(p => p.Id).ToList();
                    var ingredients = db.Ingredients.Where(p => categoryIds.Contains(p.CategoryId)).ToList();

                    clietns.ForEach(p => { p.BranchId = -1; p.CityId = -1; });
                    categories.ForEach(p => p.IsDeleted = true);
                    products.ForEach(p => p.IsDeleted = true);
                    additionalOptions.ForEach(p => p.IsDeleted = true);
                    aditionOptionItems.ForEach(p => p.IsDeleted = true);
                    productAdditionalOptions.ForEach(p => p.IsDeleted = true);
                    additionalFillgs.ForEach(p => p.IsDeleted = true);
                    productAdditionalFillings.ForEach(p => p.IsDeleted = true);

                    constructoCategories.ForEach(p => p.IsDeleted = true);
                    ingredients.ForEach(p => p.IsDeleted = true);

                    if (branch != null)
                        branch.IsDeleted = true;

                    if (setting != null)
                        setting.IsDeleted = true;

                    if (deliverySetting != null)
                        deliverySetting.IsDeleted = true;

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
                var dict = areaDeliveries.ToDictionary(p => p.Id);
                var ids = dict.Keys.ToList();
                var deliverySettingId = areaDeliveries.First().DeliverySettingId;

                using (var db = new AdminPanelContext())
                {
                    var allAreas = db.AreaDeliveryModels.Where(p => p.DeliverySettingId == deliverySettingId).ToList();
                    var updates = allAreas
                        .Where(x => ids.Contains(x.Id))
                        .ToList();
                    var newAreas = areaDeliveries
                        .Where(p => !updates.Exists(x => p.Id == x.Id))
                        .ToList();
                    var removeAreas = allAreas
                        .Where(x => !updates.Exists(p => p.Id == x.Id))
                        .ToList();

                    if (removeAreas != null && removeAreas.Any())
                    {
                        db.AreaDeliveryModels.RemoveRange(removeAreas);
                    }

                    if (updates != null)
                    {
                        updates.ForEach(p =>
                        {
                            p.NameArea = dict[p.Id].NameArea;
                            p.MinPrice = dict[p.Id].MinPrice;
                            p.DeliveryPrice = dict[p.Id].DeliveryPrice;
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
                        updateSetting.IsDelivery = setting.IsDelivery;
                        updateSetting.IsTakeYourSelf = setting.IsTakeYourSelf;
                        updateSetting.PayCard = setting.PayCard;
                        updateSetting.PayCash = setting.PayCash;
                        updateSetting.PayOnline = setting.PayOnline;
                        updateSetting.MerchantId = setting.MerchantId;
                        updateSetting.PaymentKey = setting.PaymentKey;
                        updateSetting.CreditKey = setting.CreditKey;
                        updateSetting.IsAcceptedOnlinePayCondition = setting.IsAcceptedOnlinePayCondition;
                        updateSetting.TimeDeliveryJSON = setting.TimeDeliveryJSON;
                        updateSetting.ZoneId = setting.ZoneId;
                        updateSetting.IsSoundNotify = setting.IsSoundNotify;
                        updateSetting.NotificationEmail = setting.NotificationEmail;
                        updateSetting.MaxPreorderPeriod = setting.MaxPreorderPeriod;
                        updateSetting.MinTimeProcessingOrder = setting.MinTimeProcessingOrder;
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
                    var orderNumber = db.Categories.Where(p => p.BranchId == category.BranchId && !p.IsDeleted).Count() + 1;

                    category.OrderNumber = orderNumber;
                    result = db.Categories.Add(category);
                    db.SaveChanges();

                    List<RecommendedProductModel> recommendedProducts = category.RecommendedProducts?
                        .Select(productId => new RecommendedProductModel
                        {
                            BranchId = result.BranchId,
                            CategoryId = result.Id,
                            ProductId = productId
                        })
                        .ToList();
                    SaveRecommendedProductsForCategory(result.Id, recommendedProducts);
                    result.RecommendedProducts = category.RecommendedProducts;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static void SaveRecommendedProductsForCategory(int categoryId, List<RecommendedProductModel> products)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var forRemove = db.RecommendedProducts.Where(p => p.CategoryId == categoryId);
                    if (forRemove.Count() != 0)
                        db.RecommendedProducts.RemoveRange(forRemove);

                    if (products != null && products.Any())
                    {
                        db.RecommendedProducts.AddRange(products);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static List<int> GetRecommendedProductsForCategory(int categoryId)
        {
            var productIds = new List<int>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    productIds = db.RecommendedProducts.Where(p => p.CategoryId == categoryId).Select(p => p.ProductId).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return productIds;
        }

        public static List<RecommendedProductModel> GetRecommendedProductsForCategoryByBranchId(int branchId)
        {
            var products = new List<RecommendedProductModel>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    products = db.RecommendedProducts.Where(p => p.BranchId == branchId).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return products;
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
                        var orderNumber = db.ConstructorCategories.Where(p => p.BranchId == category.BranchId && !p.IsDeleted).Count() + 1;

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

                    List<RecommendedProductModel> recommendedProducts = category.RecommendedProducts?
                        .Select(productId => new RecommendedProductModel
                        {
                            BranchId = result.BranchId,
                            CategoryId = result.Id,
                            ProductId = productId
                        })
                        .ToList();
                    SaveRecommendedProductsForCategory(result.Id, recommendedProducts);
                    result.RecommendedProducts = category.RecommendedProducts;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static ConstructorCategory SaveConstructorCategoryWihoutChangeOrderNumber(ConstructorCategory category)
        {
            ConstructorCategory result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.ConstructorCategories.Add(category);
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

                    foreach(var categoryId in result.Keys)
                    {
                        result[categoryId].RecommendedProducts = GetRecommendedProductsForCategory(categoryId);
                    }
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

                    foreach (var category in result)
                    {
                        category.RecommendedProducts = GetRecommendedProductsForCategory(category.Id);
                    }
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
                        .OrderBy(p => p.OrderNumber)
                        .ToList();

                    foreach (var category in result)
                    {
                        category.RecommendedProducts = GetRecommendedProductsForCategory(category.Id);
                    }
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
                    result.NumberAppliances = category.NumberAppliances;

                    db.SaveChanges();

                    List<RecommendedProductModel> recommendedProducts = category.RecommendedProducts?
                        .Select(productId => new RecommendedProductModel
                        {
                            BranchId = result.BranchId,
                            CategoryId = result.Id,
                            ProductId = productId
                        })
                        .ToList();
                    SaveRecommendedProductsForCategory(result.Id, recommendedProducts);
                    result.RecommendedProducts = category.RecommendedProducts;
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
                        RemoveProductByCategoryId(id);

                        success = true;

                        SaveRecommendedProductsForCategory(id, null);
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
                        .Where(p => p.CategoryId == product.CategoryId && !p.IsDeleted)
                        .Count();

                    product.OrderNumber = orderNumber;
                    result = db.Products.Add(product);
                    db.SaveChanges();

                    if (result != null && product.ProductAdditionalOptionIds != null && product.ProductAdditionalOptionIds.Any())
                    {
                        var additionalOptionOrderNamber = 1;
                        var additionalOptions = product.ProductAdditionalOptionIds
                            .Select(p => new ProductAdditionalOptionModal
                            {
                                AdditionalOptionId = p,
                                BranchId = product.BranchId,
                                OrderNumber = additionalOptionOrderNamber++,
                                ProductId = result.Id
                            }).ToList();

                        SaveProductAdditionalOptions(result.Id, additionalOptions);
                        result.ProductAdditionalOptionIds = product.ProductAdditionalOptionIds;
                    }



                    if (result != null && result.ProductAdditionalFillingIds != null && product.ProductAdditionalFillingIds.Any())
                    {
                        var additionalFillingOrderNumber = 1;
                        var additionalFillings = product.ProductAdditionalFillingIds
                            .Select(p => new ProductAdditionalFillingModal
                            {
                                AdditionalFillingId = p,
                                BranchId = product.BranchId,
                                OrderNumber = additionalFillingOrderNumber,
                                ProductId = result.Id
                            })
                            .ToList();

                        SaveProductAdditionalFilling(result.Id, additionalFillings);
                        result.ProductAdditionalFillingIds = product.ProductAdditionalFillingIds;
                    }
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
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var result = db.Products.AddRange(products).ToList();
                    db.SaveChanges();

                    if (result != null)
                    {
                        for (var i = 0; i < result.Count; ++i)
                        {
                            var productAdditionalOptionIds = products[i].ProductAdditionalOptionIds;
                            var product = result[i];

                            if (productAdditionalOptionIds != null && productAdditionalOptionIds.Any())
                            {
                                var additionalOprionOrderNamber = 1;
                                var additionalOptions = productAdditionalOptionIds
                                    .Select(p => new ProductAdditionalOptionModal
                                    {
                                        AdditionalOptionId = p,
                                        BranchId = product.BranchId,
                                        OrderNumber = additionalOprionOrderNamber++,
                                        ProductId = product.Id
                                    }).ToList();

                                SaveProductAdditionalOptions(product.Id, additionalOptions);
                            }

                            var productAdditionalFillingIds = products[i].ProductAdditionalFillingIds;
                            if (productAdditionalFillingIds != null && productAdditionalFillingIds.Any())
                            {
                                var additionalFillingOrderNumber = 1;
                                var additionalFillings = productAdditionalFillingIds
                                    .Select(p => new ProductAdditionalFillingModal
                                    {
                                        AdditionalFillingId = p,
                                        BranchId = product.BranchId,
                                        OrderNumber = additionalFillingOrderNumber,
                                        ProductId = product.Id
                                    })
                                    .ToList();

                                SaveProductAdditionalFilling(product.Id, additionalFillings);
                            }
                        }
                    }
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

                    if (result.Any())
                    {
                        var productIds = result.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(productIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            result.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(productIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            result.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<ProductModel> GetOrderProducts(IEnumerable<int> ids)
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

                    if (result.Any())
                    {
                        var productIds = result.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(productIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            result.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(productIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            result.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);
                    }
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
                        order.ApproximateDeliveryTime = data.ApproximateDeliveryTime;

                        if (data.Status == OrderStatus.Cancellation)
                            order.CommentCauseCancel = data.CommentCauseCancel;
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

                    if (result != null)
                    {
                        var productIds = new List<int> { result.Id };
                        var optionDict = GetProductAdditionalOptions(productIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            result.ProductAdditionalOptionIds = optionDict[result.Id];
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(productIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            result.ProductAdditionalFillingIds = additionalFillingsDict[result.Id];
                    }
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
                        .ToList();

                    if (result.Any())
                    {
                        var pIds = result.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(pIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            result.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(pIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            result.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<int, ProductModel> GetProductDictionary(IEnumerable<int> productIds)
        {
            Dictionary<int, ProductModel> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var products = db
                        .Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToList();

                    if (products.Any())
                    {
                        var pIds = products.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(pIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            products.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(pIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            products.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);

                        result = products.ToDictionary(p => p.Id);
                    }
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

                    if (result.Any())
                    {
                        var productIds = result.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(productIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            result.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(productIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            result.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);
                    }
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

                    if (products.Any())
                    {
                        var productIds = products.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(productIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            products.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(productIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            products.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);
                    }

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

                    if (result.Any())
                    {
                        var productIds = result.Select(p => p.Id).ToList();
                        var optionDict = GetProductAdditionalOptions(productIds);
                        if (optionDict != null && optionDict.Any())
                        {
                            result.ForEach(p => p.ProductAdditionalOptionIds = optionDict[p.Id]);
                        }

                        var additionalFillingsDict = GetProductAdditionalFillings(productIds);
                        if (additionalFillingsDict != null && additionalFillingsDict.Any())
                            result.ForEach(p => p.ProductAdditionalFillingIds = additionalFillingsDict[p.Id]);
                    }
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
                    result.ProductAdditionalInfoType = product.ProductAdditionalInfoType;
                    result.ProductAdditionalOptionIds = product.ProductAdditionalOptionIds;
                    result.ProductAdditionalFillingIds = product.ProductAdditionalFillingIds;
                    result.AllowCombinationsJSON = product.AllowCombinationsJSON;
                    result.VendorCode = product.VendorCode;
                    result.AllowCombinationsVendorCodeJSON = product.AllowCombinationsVendorCodeJSON;

                    db.SaveChanges();

                    if (result != null)
                    {
                        var additionalOprionOrderNamber = 1;
                        var additionalOptions = product.ProductAdditionalOptionIds == null ? null : product.ProductAdditionalOptionIds
                            .Select(p => new ProductAdditionalOptionModal
                            {
                                AdditionalOptionId = p,
                                BranchId = product.BranchId,
                                OrderNumber = additionalOprionOrderNamber++,
                                ProductId = result.Id
                            }).ToList();

                        SaveProductAdditionalOptions(result.Id, additionalOptions);


                        var additionalFillingOrderNumber = 1;
                        var additionalFillings = product.ProductAdditionalFillingIds == null ? null : product.ProductAdditionalFillingIds
                            .Select(p => new ProductAdditionalFillingModal
                            {
                                AdditionalFillingId = p,
                                BranchId = product.BranchId,
                                OrderNumber = additionalFillingOrderNumber,
                                ProductId = result.Id
                            })
                            .ToList();

                        SaveProductAdditionalFilling(result.Id, additionalFillings);
                    }
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
                        SaveProductAdditionalOptions(removeProduct.Id, null);
                        SaveProductAdditionalFilling(removeProduct.Id, null);

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

        public static bool RemoveProductByCategoryId(int categoryId)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var removeProducts = db.Products.Where(p => p.CategoryId == categoryId).ToList();

                    if (removeProducts != null)
                    {
                        removeProducts.ForEach(p =>
                        {
                            p.IsDeleted = true;
                            SaveProductAdditionalOptions(p.Id, null);
                            SaveProductAdditionalFilling(p.Id, null);
                        });
                        db.SaveChanges();

                        RecalcProductsOrderNumber(categoryId);
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
                        .Where(p => !p.IsDeleted)
                        .Select(p => p.BranchId)
                        .Distinct()
                        .ToList();

                    alloweCity = db
                        .Settings
                        .Where(p => !p.IsDeleted)
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
                        .Where(p => p.ClientId == clientId &&
                         p.OrderStatus != OrderStatus.Cancellation &&
                         p.OrderStatus != OrderStatus.Deleted &&
                         p.OrderStatus != OrderStatus.PendingPay)
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
                        .Where(p => p.ClientId == clientId &&
                         p.OrderStatus != OrderStatus.Cancellation &&
                         p.OrderStatus != OrderStatus.Deleted &&
                         p.OrderStatus != OrderStatus.PendingPay)
                        .Count() == 0;
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
                                    p.OrderStatus != OrderStatus.Deleted &&
                                    p.OrderStatus != OrderStatus.PendingPay &&
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
                    var deliverySetting = db.DeliverySettings.FirstOrDefault(p => p.BranchId == branchId);
                    histroyOrders = db.Orders
                        .Where(p => p.ClientId == clientId &&
                        p.BranchId == branchId &&
                        p.OrderStatus != OrderStatus.Deleted &&
                        p.OrderStatus != OrderStatus.PendingPay)
                        .OrderByDescending(p => p.Date)
                        .Take(50)
                        .OrderBy(p => p.Date)
                        .ToList();

                    if (deliverySetting != null)
                    {
                        histroyOrders.ForEach(p => 
                        {
                            if(p.ApproximateDeliveryTime.HasValue)
                                p.ApproximateDeliveryTime = TimeZoneInfo.ConvertTimeToUtc(
                                    p.ApproximateDeliveryTime.Value,
                                    TimeZoneInfo.FindSystemTimeZoneById(deliverySetting.ZoneId));
                        });
                    }

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

                    if (saved != null)
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

            if (productIds == null || !productIds.Any())
                return result;

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
                        || (DbFunctions.TruncateTime(p.StockFromDate) <= date
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

        public static Dictionary<Guid, List<int>> GetStockIdsByGuid(List<Guid> items)
        {
            Dictionary<Guid, List<int>> result = new Dictionary<Guid, List<int>>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Stocks
                        .Where(p => items.Contains(p.UniqId))
                        .GroupBy(p => p.UniqId)
                        .ToDictionary(p => p.Key, p => p.Select(x => x.Id).ToList());
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

                    if (newClient != null)
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
                    newClient = db.Clients.FirstOrDefault(p => p.PhoneNumber == client.PhoneNumber);

                    if (newClient != null)
                    {
                        newClient.UserName = client.UserName;
                        newClient.DateBirth = client.DateBirth;
                        newClient.Email = client.Email;
                        newClient.ParentReferralClientId = client.ParentReferralClientId;
                        newClient.ParentReferralCode = client.ParentReferralCode;
                        newClient.ReferralDiscount = client.ReferralDiscount;
                        newClient.VirtualMoney = client.VirtualMoney;
                        newClient.CityId = client.CityId;
                        newClient.BranchId = client.BranchId;
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
                        client.VirtualMoney = Math.Round(virtualMoney, 2);
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

            if (string.IsNullOrEmpty(email))
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
                                    p.OrderStatus != OrderStatus.Deleted &&
                                    p.OrderStatus != OrderStatus.PendingPay &&
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
                        Func<OrderModel, bool> condition = p => p.ClientId == data.ClientId &&
                        coupons.Contains(p.CouponId) &&
                        p.OrderStatus != OrderStatus.Cancellation &&
                        p.OrderStatus != OrderStatus.Deleted &&
                        p.OrderStatus != OrderStatus.PendingPay;
                        var isFirstUseCoupon = db.Orders.FirstOrDefault(condition) == null;

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
                            oldSetting.AlwaysApplyCashback = setting.AlwaysApplyCashback;

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
                        oldSetting.IsShowNewsBanner = setting.IsShowNewsBanner;
                        oldSetting.StockInteractionType = setting.StockInteractionType;
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
        public static Dictionary<int, List<IngredientModel>> GetAllDictionaryIngredientsByCategoryIds(IEnumerable<int> ids)
        {
            Dictionary<int, List<IngredientModel>> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db
                        .Ingredients
                        .Where(p => ids.Contains(p.CategoryId) && !p.IsDeleted)
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

        public static PushMessageModel SavePushMessage(PushMessageModel message)
        {
            PushMessageModel savedMessage = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    savedMessage = db.PushMessages.Add(message);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return savedMessage;
        }

        public static List<PushMessageModel> GetPushMessage(int branchId, int pageNumber, int pageSize)
        {
            List<PushMessageModel> historyMessages = null;

            if (branchId < 1 ||
                pageNumber < 1 ||
                pageSize < 1)
                return historyMessages;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    historyMessages = db.PushMessages
                        .Where(p => p.BranchId == branchId)
                        .OrderByDescending(p => p.Date)
                        .Skip(pageSize * (pageNumber - 1))
                        .Take(pageSize)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return historyMessages;
        }

        public static int GetCountPushMessage(int branchId)
        {
            int count = 0;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    count = db.PushMessages
                        .Where(p => p.BranchId == branchId)
                       .Count();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return count;
        }

        public static int GetCountPushMessageByDate(int branchId, DateTime date)
        {
            int count = 0;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    count = db.PushMessages
                        .Where(p => p.BranchId == branchId &&
                        DbFunctions.TruncateTime(p.Date) == date.Date)
                       .Count();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return count;
        }

        public static void AddOrUpdateDevice(FCMDeviceModel device)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    void addTokennotRegisterClient()
                    {
                        var notRegisteDevice = db.FCMDevices.FirstOrDefault(p => p.Token == device.Token);
                        if (notRegisteDevice != null)
                        {
                            notRegisteDevice.Platform = device.Platform;
                            notRegisteDevice.Token = device.Token;
                            notRegisteDevice.ClientId = device.ClientId;
                            notRegisteDevice.BranchId = device.BranchId;

                            db.SaveChanges();
                        }
                        else
                        {
                            db.FCMDevices.Add(device);
                            db.SaveChanges();
                        }
                    }

                    if (device.ClientId > 0)
                    {
                        var updDevice = db.FCMDevices.FirstOrDefault(p => p.ClientId == device.ClientId);

                        if (updDevice != null)
                        {
                            updDevice.Platform = device.Platform;
                            updDevice.Token = device.Token;
                            updDevice.BranchId = device.BranchId;

                            db.SaveChanges();
                        }
                        else
                        {
                            addTokennotRegisterClient();
                        }
                    }
                    else
                    {
                        addTokennotRegisterClient();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static List<string> GetDeviceTokens(int branchId)
        {
            var tokens = new List<string>();
            try
            {
                using (var db = new AdminPanelContext())
                {
                    tokens = db.FCMDevices.Where(p => p.BranchId == branchId).Select(p => p.Token).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return tokens;
        }

        public static bool IsUseStockWithTriggerBirthday(List<int> stockIds, int branchId)
        {
            bool isUse = false;

            if (stockIds == null || !stockIds.Any())
                return isUse;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    isUse = db.Stocks.Where(p => stockIds.Contains(p.Id) &&
                    p.ConditionType == StockConditionTriggerType.HappyBirthday)
                    .Count() > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return isUse;
        }

        public static void FixUseStockWithBirthday(int clientId, DateTime dateUse)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var client = GetClient(clientId);
                    if (client == null)
                        throw new Exception($"Клиент {clientId} не найден");

                    var newFixUse = new FixBirthday
                    {
                        ClientId = client.Id,
                        DateUse = dateUse.Date,
                        DateBirth = client.DateBirth.Value
                    };

                    var fixUse = GetFixBirthday(clientId, dateUse.Date);
                    if (fixUse == null)
                    {
                        db.FixBirthdays.Add(newFixUse);
                    }
                    else
                    {
                        var existingData = db.FixBirthdays.FirstOrDefault(p => p.Id == fixUse.Id);
                        existingData.DateUse = dateUse.Date;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static FixBirthday GetFixBirthday(int clientId, DateTime useDate)
        {
            FixBirthday fixBirthday = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var client = GetClient(clientId);
                    if (client == null)
                        throw new Exception($"Клиент {clientId} не найден");

                    if (client.DateBirth != null)
                    {
                        var dateNow = useDate.Date;

                        fixBirthday = db.FixBirthdays.FirstOrDefault(p => p.ClientId == clientId &&
                        DbFunctions.DiffYears(p.DateUse, dateNow) == 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return fixBirthday;
        }

        public static bool AllowUseStockWithBirthday(int clientId)
        {
            var isAllow = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var client = GetClient(clientId);
                    if (client == null)
                        throw new Exception($"Клиент {clientId} не найден");

                    if (client.DateBirth != null)
                    {
                        var dateNow = DateTime.Now.Date;

                        var fixUse = GetFixBirthday(clientId, dateNow);

                        isAllow = fixUse == null || fixUse.DateBirth.Date == client.DateBirth.Value.Date;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return isAllow;
        }

        public static PromotionNewsModel SavePromotionNews(PromotionNewsModel promotionNews)
        {
            PromotionNewsModel news = promotionNews;

            if (news == null)
                return news;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (news.Id > 0)
                    {
                        var oldNews = db.PromotionNews.FirstOrDefault(p => p.Id == news.Id);

                        if (oldNews != null)
                        {
                            oldNews.Title = news.Title;
                            oldNews.Description = news.Description;
                            oldNews.Image = news.Image;
                        }
                    }
                    else
                    {
                        news = db.PromotionNews.Add(news);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return news;
        }

        public static bool RemovePromotionNews(int niewsId)
        {
            bool isRemoved = false;

            if (niewsId < 1)
                return isRemoved;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var oldNews = db.PromotionNews.FirstOrDefault(p => p.Id == niewsId);

                    if (oldNews != null)
                    {
                        oldNews.IsDeleted = true;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return isRemoved;
        }

        public static List<PromotionNewsModel> GetPromotionNews(int branchId)
        {
            List<PromotionNewsModel> news = new List<PromotionNewsModel>();

            if (branchId < 1)
                return news;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    news = db.PromotionNews
                        .Where(p => p.BranchId == branchId &&
                        !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return news;
        }

        public static bool RemoveProductAdditionalOptionById(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (id > 0)
                    {
                        var value = db.AdditionalOptions.FirstOrDefault(p => p.Id == id);
                        value.IsDeleted = true;

                        success = RemoveProductAdditionOptionItemsByOptionId(id);
                        if (success)
                            db.SaveChanges();

                        RemoveProductAdditionalOptionByOptionId(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return success;
        }

        public static bool RemoveProductAdditionOptionItemsByOptionId(int id)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (id > 0)
                    {
                        var oldItems = db.AdditionOptionItems.Where(p => p.AdditionOptionId == id);
                        foreach (var item in oldItems)
                            item.IsDeleted = true;

                        if (oldItems.Count() > 0)
                            db.SaveChanges();

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

        public static AdditionalOption SaveProductAdditionalOption(AdditionalOption additionalOption)
        {
            AdditionalOption result = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    AdditionalOption value;
                    if (additionalOption.Id > 0)
                    {
                        value = db.AdditionalOptions.FirstOrDefault(p => p.Id == additionalOption.Id);

                        value.Name = additionalOption.Name;
                        value.Items = additionalOption.Items;
                    }
                    else
                        value = db.AdditionalOptions.Add(additionalOption);

                    db.SaveChanges();
                    result = value;

                    additionalOption.Items.ForEach(p =>
                    {
                        p.AdditionOptionId = value.Id;
                        p.BranchId = value.BranchId;
                    });

                    List<AdditionOptionItem> additionOptionItems = SaveProductAdditionOptionItemsAndRemoveOld(result.Id, additionalOption.Items);
                    result.Items = additionOptionItems;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = null;
            }

            return result;
        }

        public static List<AdditionOptionItem> SaveProductAdditionOptionItemsAndRemoveOld(int additionalOptionId, List<AdditionOptionItem> items)
        {
            var result = new List<AdditionOptionItem>();

            if (items != null)
            {
                try
                {
                    using (var db = new AdminPanelContext())
                    {

                        var allItems = db.AdditionOptionItems.Where(p => p.AdditionOptionId == additionalOptionId).ToList();
                        foreach (var item in allItems)
                        {
                            if (!items.Exists(p => p.Id == item.Id))
                                item.IsDeleted = true;
                        }

                        foreach (var option in items)
                        {
                            AdditionOptionItem additionOptionItem;
                            if (option.Id > 0)
                            {
                                additionOptionItem = db.AdditionOptionItems.FirstOrDefault(p => p.Id == option.Id);

                                additionOptionItem.Name = option.Name;
                                additionOptionItem.AdditionalInfo = option.AdditionalInfo;
                                additionOptionItem.Price = option.Price;
                                additionOptionItem.IsDefault = option.IsDefault;
                            }
                            else
                                additionOptionItem = db.AdditionOptionItems.Add(option);

                            result.Add(additionOptionItem);
                        }

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                    result = null;
                }
            }
            return result;
        }

        public static List<AdditionOptionItem> SaveProductAdditionOptionItems(List<AdditionOptionItem> items)
        {
            var result = new List<AdditionOptionItem>();

            if (items != null)
            {
                try
                {
                    using (var db = new AdminPanelContext())
                    {
                        foreach (var option in items)
                        {
                            AdditionOptionItem additionOptionItem;
                            if (option.Id > 0)
                            {
                                additionOptionItem = db.AdditionOptionItems.FirstOrDefault(p => p.Id == option.Id);

                                additionOptionItem.Name = option.Name;
                                additionOptionItem.AdditionalInfo = option.AdditionalInfo;
                                additionOptionItem.Price = option.Price;
                                additionOptionItem.IsDefault = option.IsDefault;
                            }
                            else
                                additionOptionItem = db.AdditionOptionItems.Add(option);

                            result.Add(additionOptionItem);
                        }

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                    result = null;
                }
            }
            return result;
        }

        public static List<AdditionalOption> GetAllProductAdditionalOptionByBranchId(int branchId)
        {
            List<AdditionalOption> additionalOptions = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var additionOptionItemDict = GetAllProductAdditionOptionItemByBranchId(branchId);
                    var options = db.AdditionalOptions.Where(p => p.BranchId == branchId && !p.IsDeleted);

                    if (options != null && additionOptionItemDict != null)
                    {
                        additionalOptions = new List<AdditionalOption>();

                        foreach (var option in options)
                        {
                            option.Items = additionOptionItemDict[option.Id];

                            additionalOptions.Add(option);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                additionalOptions = null;
            }

            return additionalOptions;
        }

        public static Dictionary<int, List<AdditionOptionItem>> GetAllProductAdditionOptionItemByBranchId(int branchId)
        {
            Dictionary<int, List<AdditionOptionItem>> additionOptionItemDict = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    additionOptionItemDict = db.AdditionOptionItems.Where(p => p.BranchId == branchId && !p.IsDeleted)
                        .GroupBy(p => p.AdditionOptionId)
                        .ToDictionary(p => p.Key, p => p.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                additionOptionItemDict = null;
            }

            return additionOptionItemDict;
        }

        public static Dictionary<int, List<AdditionOptionItem>> GetAdditionOptionItemByIds(List<int> itemIds)
        {
            Dictionary<int, List<AdditionOptionItem>> additionOptionItemDict = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    additionOptionItemDict = db.AdditionOptionItems.Where(p => itemIds.Contains(p.Id))
                        .GroupBy(p => p.AdditionOptionId)
                        .ToDictionary(p => p.Key, p => p.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                additionOptionItemDict = null;
            }

            return additionOptionItemDict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns>key - procut id, value additional option id</returns>
        public static Dictionary<int, List<int>> GetProductAdditionalOptions(List<int> productIds)
        {
            var productAdditionOptionsDict = new Dictionary<int, List<int>>();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    productAdditionOptionsDict = db.ProductAdditionalOptions.Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                        .GroupBy(p => p.ProductId)
                        .ToDictionary(
                        p => p.Key,
                        p => p.OrderBy(x => x.OrderNumber)
                        .Select(x => x.AdditionalOptionId)
                        .ToList());

                    productIds.ForEach(p =>
                    {
                        List<int> additionalOptionIds = null;

                        if (productAdditionOptionsDict.TryGetValue(p, out additionalOptionIds))
                        {
                            if (additionalOptionIds == null)
                                productAdditionOptionsDict[p] = new List<int>();
                        }
                        else
                            productAdditionOptionsDict.Add(p, new List<int>());
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return productAdditionOptionsDict;
        }

        public static Dictionary<int, AdditionalOption> GetProductAdditionalOptionsByIds(List<int> additionalOptionIds, List<int> additionalOptionItemIds)
        {
            var additionOptionsDict = new Dictionary<int, AdditionalOption>();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var additionalOptions = db.AdditionalOptions.Where(p => additionalOptionIds.Contains(p.Id));
                    var additionOptionItemDict = GetAdditionOptionItemByIds(additionalOptionItemIds);

                    if (additionalOptions != null && additionOptionItemDict != null)
                    {
                        foreach (var option in additionalOptions)
                        {
                            option.Items = additionOptionItemDict[option.Id];

                            additionOptionsDict.Add(option.Id, option);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return additionOptionsDict;
        }

        public static void SaveProductAdditionalOptions(int productId, List<ProductAdditionalOptionModal> productAdditionalOptions)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var oldOptions = db.ProductAdditionalOptions.Where(p => p.ProductId == productId && !p.IsDeleted);
                    foreach (var item in oldOptions)
                        item.IsDeleted = true;

                    db.SaveChanges();

                    if (productAdditionalOptions != null && productAdditionalOptions.Any())
                    {
                        db.ProductAdditionalOptions.AddRange(productAdditionalOptions);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static void RemoveProductAdditionalOptionByOptionId(int id)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var oldOptions = db.ProductAdditionalOptions.Where(p => p.AdditionalOptionId == id && !p.IsDeleted);
                    foreach (var item in oldOptions)
                        item.IsDeleted = true;

                    if (oldOptions.Count() > 0)
                        db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static AdditionalFilling SaveAdditionalFilling(AdditionalFilling additionalFilling)
        {
            AdditionalFilling result = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    AdditionalFilling value;
                    if (additionalFilling.Id > 0)
                    {
                        value = db.AdditionalFillings.FirstOrDefault(p => p.Id == additionalFilling.Id);

                        value.Name = additionalFilling.Name;
                        value.Price = additionalFilling.Price;
                        value.VendorCode = additionalFilling.VendorCode;
                    }
                    else
                        value = db.AdditionalFillings.Add(additionalFilling);

                    db.SaveChanges();
                    result = value;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = null;
            }

            return result;
        }

        public static void SaveProductAdditionalFilling(int productId, List<ProductAdditionalFillingModal> productAdditionalFillings)
        {
            try
            {
                using (var db = new AdminPanelContext())
                {
                    var oldItems = db.ProductAdditionalFillings.Where(p => p.ProductId == productId && !p.IsDeleted);
                    foreach (var item in oldItems)
                        item.IsDeleted = true;

                    if (oldItems.Count() > 0)
                        db.SaveChanges();

                    if (productAdditionalFillings != null && productAdditionalFillings.Any())
                    {
                        db.ProductAdditionalFillings.AddRange(productAdditionalFillings);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static bool RemoveAdditionalFilling(int id)
        {
            bool success;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    AdditionalFilling value = db.AdditionalFillings.FirstOrDefault(p => p.Id == id);
                    value.IsDeleted = true;

                    success = RemoveProductAdditionalFilling(id);

                    if (success)
                        db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                success = false;
            }

            return success;
        }

        public static bool RemoveProductAdditionalFilling(int id)
        {
            bool success;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var oldItems = db.ProductAdditionalFillings.Where(p => p.AdditionalFillingId == id);
                    foreach (var item in oldItems)
                        item.IsDeleted = true;

                    if (oldItems.Count() > 0)
                        db.SaveChanges();

                    success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                success = false;
            }

            return success;
        }

        public static Dictionary<int, List<int>> GetProductAdditionalFillings(List<int> productIds)
        {
            var productAdditionFillingsDict = new Dictionary<int, List<int>>();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    productAdditionFillingsDict = db.ProductAdditionalFillings.Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                        .GroupBy(p => p.ProductId)
                        .ToDictionary(
                        p => p.Key,
                        p => p.OrderBy(x => x.OrderNumber)
                        .Select(x => x.AdditionalFillingId)
                        .ToList());

                    productIds.ForEach(p =>
                    {
                        List<int> additionalFillingIds = null;

                        if (productAdditionFillingsDict.TryGetValue(p, out additionalFillingIds))
                        {
                            if (additionalFillingIds == null)
                                productAdditionFillingsDict[p] = new List<int>();
                        }
                        else
                            productAdditionFillingsDict.Add(p, new List<int>());
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return productAdditionFillingsDict;
        }

        public static List<AdditionalFilling> GetAllAdditionalFillingsByBranchId(int branchId)
        {
            List<AdditionalFilling> additionalfillings = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    additionalfillings = db.AdditionalFillings.Where(p => p.BranchId == branchId && !p.IsDeleted)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                additionalfillings = null;
            }

            return additionalfillings;
        }

        public static Dictionary<int, AdditionalFilling> GetAdditionalFillingsByIds(List<int> ids)
        {
            Dictionary<int, AdditionalFilling> additionalfillings = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    additionalfillings = db.AdditionalFillings.Where(p => ids.Contains(p.Id)).ToDictionary(p => p.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                additionalfillings = null;
            }

            return additionalfillings;
        }
    }
}