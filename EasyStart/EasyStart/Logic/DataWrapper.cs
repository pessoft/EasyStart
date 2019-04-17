using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class DataWrapper
    {
        public static Dictionary<int,SettingModel> GetAllSettingDictionary()
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
                    branch = db.Branches.FirstOrDefault(p => p.Login == login && p.Password == password );
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

        public static bool SaveSetting(SettingModel setting)
        {
            var success = false;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    if (setting.Id != 0)
                    {
                        var updateSetting = db.Settings.FirstOrDefault(p => p.Id == setting.Id);
                        updateSetting.BranchId = setting.BranchId;
                        updateSetting.CityId = setting.CityId;
                        updateSetting.FreePriceDelivery = setting.FreePriceDelivery;
                        updateSetting.HomeNumber = setting.HomeNumber;
                        updateSetting.PriceDelivery = setting.PriceDelivery;
                        updateSetting.Street = setting.Street;
                        updateSetting.TimeClose = setting.TimeClose;
                        updateSetting.TimeOpen = setting.TimeOpen;
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
    }
}