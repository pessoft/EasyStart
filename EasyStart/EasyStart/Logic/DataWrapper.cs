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
                    alloweCity = db
                        .Settings
                        .Select(p => p.CityId)
                        .ToList();
                }
            }
            catch (Exception ex)
            { }

            return alloweCity;
        }
    }
}