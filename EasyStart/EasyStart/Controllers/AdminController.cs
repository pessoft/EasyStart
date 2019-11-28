using EasyStart.Logic;
using EasyStart.Models;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EasyStart.Controllers
{
    [RedirectingAction]
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize]
        public ActionResult AdminPanel()
        {
            try
            {


                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var typeBranch = DataWrapper.GetBranchType(branchId);
                var converter = new ConverterBranchSetting();
                var branchViewvs = DataWrapper.GetAllSettingDictionary()
                    .Where(p => p.Value.CityId > 0 && p.Key != branchId)
                    .ToDictionary(p => p.Key,
                                  p => CityHelper.GetCity(p.Value.CityId));

                var deliverySetting = DataWrapper.GetDeliverySetting(branchId); ;

                ViewBag.Zones = DateTimeHepler.GetDisplayDictionary();
                ViewBag.CurrentBranch = branchId;
                ViewBag.BranchViews = branchViewvs != null && branchViewvs.Any() ? branchViewvs : null;
                ViewBag.City = CityHelper.City;
                ViewBag.Setting = DataWrapper.GetSetting(branchId); ;
                ViewBag.DeliverySetting = deliverySetting;
                ViewBag.DeliveryTimeTable = WeeklyDayHelper.ConvertTimeDeliveryToViev(deliverySetting?.TimeDelivery);
                ViewBag.TypeBranch = typeBranch;
                ViewBag.ZoneId = deliverySetting == null ? DateTimeHepler.DEFAULT_ZONE_ID : deliverySetting.ZoneId;
                ViewBag.YearsWork = DateTime.Now.Year == 2019 ? DateTime.Now.Year.ToString() : $"2019 - {DateTime.Now.Year}";
                ViewBag.AreaDeliveris = deliverySetting != null &&
                                        deliverySetting.AreaDeliveries != null &&
                                        deliverySetting.AreaDeliveries.Any() ?
                                        JsonConvert.SerializeObject(deliverySetting.AreaDeliveries) :
                                        JsonConvert.SerializeObject(new List<AreaDeliveryModel>());

                var products = DataWrapper.GetAllProductsVisibleDictionary(branchId);
                ViewBag.ProductsForPromotion = products != null && products.Any() ?
                                        JsonConvert.SerializeObject(products) :
                                        JsonConvert.SerializeObject(new List<AreaDeliveryModel>());

                var categoryDictionary = DataWrapper.GetCategoriesVisible(branchId)
                    .GroupBy(p => p.Id).
                    ToDictionary(p => p.Key, p => p.First().Name);
                ViewBag.CategoryDictionary = categoryDictionary != null && categoryDictionary.Any() ?
                                        JsonConvert.SerializeObject(categoryDictionary) :
                                        JsonConvert.SerializeObject(new List<AreaDeliveryModel>());
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            } 

            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult UploadImage()
        {
            var result = new JsonResultModel();

            try
            {
                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];
                    if (upload != null)
                    {
                        string fileName = System.IO.Path.GetFileName(upload.FileName);
                        string ext = fileName.Substring(fileName.LastIndexOf("."));
                        string newFileName = String.Format(@"{0}{1}", System.Guid.NewGuid(), ext);

                        upload.SaveAs(Server.MapPath("~/Images/Products/" + newFileName));

                        result.Success = true;
                        result.URL = $"../Images/Products/{newFileName}";
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При загрузки изображения что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveSetting(SettingModel setting)
        {
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            setting.BranchId = branchId;
            var successSave = DataWrapper.SaveSetting(setting);
            var result = new JsonResultModel();

            if (successSave)
            {
                result.Success = successSave;
            }
            else
            {
                result.ErrorMessage = "При сохранении натсройки что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveDeliverySetting(DeliverySettingModel setting)
        {
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            setting.BranchId = branchId;
            var successSave = DataWrapper.SaveDeliverySetting(setting);
            var result = new JsonResultModel();

            if (successSave)
            {
                result.Success = successSave;
            }
            else
            {
                result.ErrorMessage = "При сохранении натсройки что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddCategory(CategoryModel category)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(category.Image)))
            {
                category.Image = "/images/default-image.jpg";
            }
            else if (category.Id > 0)
            {
                var oldImage = DataWrapper.GetCategoryImage(category.Id);

                if (oldImage != category.Image &&
                   System.IO.File.Exists(Server.MapPath(oldImage)))
                {
                    System.IO.File.Delete(Server.MapPath(oldImage));
                }
            }

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);

            category.BranchId = branchId;
            category = DataWrapper.SaveCategory(category);

            if (category != null)
            {
                result.Data = category;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении категории что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddProduct(ProductModel product)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(product.Image)))
            {
                product.Image = "/images/default-image.jpg";
            }
            else if (product.Id > 0)
            {
                var oldImage = DataWrapper.GetProductImage(product.Id);

                if (oldImage != product.Image &&
                   System.IO.File.Exists(Server.MapPath(oldImage)))
                {
                    System.IO.File.Delete(Server.MapPath(oldImage));
                }
            }

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);

            product.BranchId = branchId;
            product = DataWrapper.SaveProduct(product);

            if (product != null)
            {
                result.Data = product;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении продукта что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddBranch(NewBranchModel newBranch)
        {
            var result = new JsonResultModel();
            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var typeBranch = DataWrapper.GetBranchType(branchId);
                var newBranchId = DataWrapper.GetBranchId(newBranch.Login);

                if (typeBranch != TypeBranch.MainBranch)
                {
                    result.ErrorMessage = "Вы не можете добавлять отделения";
                    return Json(result);
                }

                var branch = new BranchModel
                {
                    Login = newBranch.Login,
                    Password = newBranch.Password,
                    TypeBranch = Logic.TypeBranch.SubBranch
                };


                if (newBranchId == -1)
                {
                    var success = DataWrapper.SaveBranch(branch);
                    if (!success)
                    {
                        result.ErrorMessage = "При сохранении что то пошло не так";
                    }
                    else
                    {
                        newBranchId = DataWrapper.GetBranchId(newBranch.Login);
                        var setting = new SettingModel
                        {
                            BranchId = newBranchId,
                            CityId = newBranch.CityId
                        };

                        DataWrapper.SaveSetting(setting);

                        var baseBrachClone = new BrachClone(Server, branchId, newBranchId);
                        baseBrachClone.Clone();

                        var converter = new ConverterBranchSetting();
                        var branchView = converter.GetBranchSettingViews(branch, setting, typeBranch);

                        result.Data = branchView;
                        result.Success = true;
                    }
                }
                else
                {
                    result.ErrorMessage = "Учетная запись с таким логином уже существует";
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }


            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveBranch(int id)
        {
            var result = new JsonResultModel();
            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var typeBranch = DataWrapper.GetBranchType(branchId);

                if (branchId == id)
                {
                    result.ErrorMessage = "Самоудаление не возможно";
                    return Json(result);
                }

                if (typeBranch != TypeBranch.MainBranch)
                {
                    result.ErrorMessage = "Вы не можете удалять отделения";
                }
                else
                {
                    var success = DataWrapper.RemoveBranch(id);

                    if (success)
                    {
                        result.Success = success;
                    }
                    else
                    {
                        result.ErrorMessage = "Отдедение не удалено";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }


            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadBranchList()
        {
            var result = new JsonResultModel();

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var typeBranch = DataWrapper.GetBranchType(branchId);
            var converter = new ConverterBranchSetting();
            var branchViewvs = converter.GetBranchSettingViews(
                DataWrapper.GetAllBranch(),
                DataWrapper.GetAllSettingDictionary(),
                typeBranch);

            result.Data = branchViewvs;
            result.Success = true;

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadCategoryList()
        {
            var result = new JsonResultModel();
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var categories = DataWrapper.GetCategories(branchId);

            if (categories != null)
            {
                result.Data = categories;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки категорий что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveCategory(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemoveCategory(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Категория не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateCategory(CategoryModel category)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(category.Image)))
            {
                category.Image = "/images/default-image.jpg";
            }
            else if (category.Id > 0)
            {
                var oldImage = DataWrapper.GetCategoryImage(category.Id);

                if (oldImage != category.Image &&
                    System.IO.File.Exists(Server.MapPath(oldImage)))
                {
                    System.IO.File.Delete(Server.MapPath(oldImage));
                }
            }

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);

            category.BranchId = branchId;
            var updateCategory = DataWrapper.UpdateCategory(category);

            if (updateCategory != null)
            {
                result.Data = updateCategory;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "Категория не обновлена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadProductList(int idCategory)
        {
            var result = new JsonResultModel();
            var products = DataWrapper.GetProducts(idCategory);

            if (products != null)
            {
                result.Data = products;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки продуктов что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveProduct(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemoveProduct(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Продукт не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateProduct(ProductModel product)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(product.Image)))
            {
                product.Image = "/images/default-image.jpg";
            }
            else if (product.Id > 0)
            {
                var oldImage = DataWrapper.GetProductImage(product.Id);

                if (oldImage != product.Image &&
                    System.IO.File.Exists(Server.MapPath(oldImage)))
                {
                    System.IO.File.Delete(Server.MapPath(oldImage));
                }
            }

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);

            product.CategoryId = branchId;
            var updateProduct = DataWrapper.UpdateProduct(product);

            if (updateProduct != null)
            {
                result.Data = updateProduct;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "Продукт не обновлена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveStock(StockModel stock)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(stock.Image)))
            {
                stock.Image = "/images/default-image.jpg";
            }

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            stock.BranchId = branchId;
            stock.IsDeleted = false;
            stock = DataWrapper.SaveStock(stock);

            if (stock != null)
            {
                result.Data = stock;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении акции что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveStock(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemoveStock(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Акция не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadStockList()
        {
            var result = new JsonResultModel();
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var stock = DataWrapper.GetStocks(branchId);

            if (stock != null)
            {
                result.Data = stock;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки акций что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public void UpdateOrderNumberCategory(List<UpdaterOrderNumber> data)
        {
            if (data != null && data.Any())
            {
                DataWrapper.UpdateOrderNumberCategory(data);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateOrderNumberProducts(List<UpdaterOrderNumber> data)
        {
            if (data != null && data.Any())
            {
                DataWrapper.UpdateOrderNumberProducts(data);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateVisibleCategory(UpdaterVisible data)
        {
            if (data != null && data.Id > 0)
            {
                DataWrapper.UpdateVisibleCategory(data);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateVisibleProduct(UpdaterVisible data)
        {
            if (data != null && data.Id > 0)
            {
                DataWrapper.UpdateVisibleProduct(data);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadProductReviews(int productId)
        {
            var result = new JsonResultModel();
            var reviews = DataWrapper.GetProductReviews(productId);

            if (reviews != null)
            {
                result.Data = reviews;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки отзывов что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public void UpdateVisibleReview(UpdaterVisible data)
        {
            if (data != null && data.Id > 0)
            {
                DataWrapper.UpdateVisibleReview(data);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadOrders(List<int> brnachIds)
        {
            var result = new JsonResultModel();
            var orders = DataWrapper.GetOrders(brnachIds);
            var todayData = DataWrapper.GetDataOrdersByDate(brnachIds, DateTime.Now);

            if (orders != null)
            {
                result.Data = new { Orders = orders, TodayData = todayData };
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки заказов, что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadHistoryOrders(HistoryOrderFilter filter)
        {
            var result = new JsonResultModel();
            var orders = DataWrapper.GetHistoryOrders(filter.BranchIds, filter.StartDate, filter.EndDate);

            if (orders != null)
            {
                result.Data = orders;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки заказов, что то пошло не так";
            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public JsonResult LoadOrderProducts(List<int> ids)
        {
            var result = new JsonResultModel();
            var products = DataWrapper.GetOrderProducts(ids);

            if (products != null)
            {
                var idsDict = products
                .Select(p => p.CategoryId)
                .Distinct()
                .ToList();
                var categoryDict = DataWrapper.GetCategories(idsDict);
                var dataResult = products
                    .GroupBy(p => p.CategoryId)
                    .Select(p => new
                    {
                        CategoryId = p.Key,
                        CategoryName = categoryDict[p.Key].Name,
                        Products = p
                    })
                    .ToList();

                result.Data = dataResult;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки продуктов что то пошло не так";
            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public void UpdateSatsusOrder(UpdaterOrderStatus data)
        {
            if (data.Status == OrderStatus.Processing)
            {
                return;
            }

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                var date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                data.DateUpdate = date;

                DataWrapper.UpdateStatusOrder(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetTodayOrderData(List<int> brnachIds)
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                var date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                var data = DataWrapper.GetDataOrdersByDate(brnachIds, date);

                result.Data = data;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadCoupons()
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var coupons = DataWrapper.GetCoupons(branchId);

                result.Data = coupons;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveCoupon(CouponModel newCoupon)
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                newCoupon.BranchId = branchId;
                newCoupon.IsDeleted = false;

                var coupon = DataWrapper.SaveCoupon(newCoupon);

                result.Data = coupon;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveCoupon(int id)
        {
            var result = new JsonResultModel();

            try
            {
                var success = DataWrapper.RemoveCoupon(id);

                result.Data = success;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadCashbackPartnerSettings()
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var cashbackSetting = DataWrapper.GetPromotionCashbackSetting(branchId);
                var partnersSetting = DataWrapper.GetPromotionPartnerSetting(branchId);

                result.Data = new { CashbackSetting = cashbackSetting, PartnersSetting = partnersSetting };
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SavePromotionCashbackSetting (PromotionCashbackSetting setting)
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                setting.BranchId = branchId;

                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                setting.DateSave = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

                var newSetting = DataWrapper.SavePromotionCashbackSetting(setting);

                result.Data = newSetting;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SavePromotionPartnerSetting(PromotionPartnerSetting setting)
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                setting.BranchId = branchId;

                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                setting.DateSave = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

                var newSetting = DataWrapper.SavePromotionPartnerSetting(setting);

                result.Data = newSetting;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadPromotionSettings()
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var settings = DataWrapper.LoadPromotionSettings(branchId);

                result.Data = settings;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SavePromotionSettings(List<PromotionSectionSetting> settings)
        {
            var result = new JsonResultModel();

            try
            {
                if (settings == null || !settings.Any())
                    throw new Exception("Пустая настрока");

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                settings.ForEach(p => p.BranchId = branchId);

                var newSettings = DataWrapper.SavePromotionSettings(settings);

                result.Data = newSettings;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }
    }
}