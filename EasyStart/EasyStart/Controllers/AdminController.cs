using EasyStart.Logic;
using EasyStart.Logic.FCM;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

                if (branchId == -1)
                {
                    FormsAuthentication.SignOut();
                    return RedirectToAction("AdminLogin", "Home");
                }

                var typeBranch = DataWrapper.GetBranchType(branchId);
                var converter = new ConverterBranchSetting();
                var branchViewvs = DataWrapper.GetAllSettingDictionary()
                    .Where(p => p.Value.CityId > 0 && p.Key != branchId)
                    .ToDictionary(p => p.Key,
                                  p => CityHelper.GetCity(p.Value.CityId));

                var deliverySetting = DataWrapper.GetDeliverySetting(branchId); ;

                var settingsValidator = new SettingsValidator();

                ViewBag.Zones = DateTimeHepler.GetDisplayDictionary();
                ViewBag.CurrentBranch = branchId;
                ViewBag.BranchViews = branchViewvs != null && branchViewvs.Any() ? branchViewvs : null;
                ViewBag.City = CityHelper.City;
                ViewBag.Setting = DataWrapper.GetSetting(branchId);
                ViewBag.IsValidSetting = settingsValidator.IsValidSetting(ViewBag.Setting);
                ViewBag.DeliverySetting = deliverySetting;
                ViewBag.IsValidDeliverySetting = settingsValidator.IsValidDeliverySetting(deliverySetting);
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
                    .Where(p => p.CategoryType == CategoryType.Default)
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
                        result.Success = false;
                        result.ErrorMessage = " Не удалось загрузить изображение";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При загрузки изображения что-то пошло не так";
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
                result.ErrorMessage = "При сохранении натсройки что-то пошло не так...";
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
                result.ErrorMessage = "При сохранении натсройки что-то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddCategory(CategoryModel category)
        {
            var result = new JsonResultModel();
            var defaultImage = "../Images/default-image.jpg";

            if (!System.IO.File.Exists(Server.MapPath(category.Image)))
            {
                category.Image = defaultImage;
            }
            else if (category.Id > 0)
            {
                var oldImage = DataWrapper.GetCategoryImage(category.Id);

                if (oldImage != category.Image
                    && oldImage != defaultImage
                   && System.IO.File.Exists(Server.MapPath(oldImage)))
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
                result.ErrorMessage = "При добавлении категории что-то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddProduct(ProductModel product)
        {
            var result = new JsonResultModel();
            var defaultImage = "../Images/default-image.jpg";
            if (!System.IO.File.Exists(Server.MapPath(product.Image)))
            {
                product.Image = defaultImage;
            }
            else if (product.Id > 0)
            {
                var oldImage = DataWrapper.GetProductImage(product.Id);

                if (oldImage != product.Image
                    && oldImage != defaultImage
                    && System.IO.File.Exists(Server.MapPath(oldImage)))
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
                result.ErrorMessage = "При добавлении продукта что-то пошло не так...";
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
                    result.ErrorMessage = "Вы не можете добавлять филиалы";
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
                    var savedBranch = DataWrapper.SaveBranch(branch);
                    if (savedBranch == null)
                    {
                        result.ErrorMessage = "При сохранении что-то пошло не так";
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

                        new PromotionDefaultSetting(newBranchId).SaveSettings();

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
                    result.ErrorMessage = "Вы не можете удалять филиалы";
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
                result.ErrorMessage = "При загрузки категорий что-то пошло не так";
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
            var defaultImage = "../Images/default-image.jpg";

            try
            {
                if (!System.IO.File.Exists(Server.MapPath(category.Image)))
                {
                    category.Image = defaultImage;
                }
                else if (category.Id > 0)
                {
                    var oldImage = DataWrapper.GetCategoryImage(category.Id);

                    if (oldImage != category.Image
                        && oldImage != defaultImage
                        && System.IO.File.Exists(Server.MapPath(oldImage)))
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
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При загрузке изображения что-то пошло не так...";
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
                result.ErrorMessage = "При загрузки продуктов что-то пошло не так";
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
            var defaultImage = "../Images/default-image.jpg";

            if (!System.IO.File.Exists(Server.MapPath(product.Image)))
            {
                product.Image = defaultImage;
            }
            else if (product.Id > 0)
            {
                var oldImage = DataWrapper.GetProductImage(product.Id);

                if (oldImage != product.Image
                    && oldImage != defaultImage
                    && System.IO.File.Exists(Server.MapPath(oldImage)))
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
        public JsonResult SaveNews(PromotionNewsModel promotionNews)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(promotionNews.Image)))
            {
                promotionNews.Image = "../Images/default-image.jpg";
            }

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            promotionNews.BranchId = branchId;
            promotionNews.IsDeleted = false;
            promotionNews = DataWrapper.SavePromotionNews(promotionNews);

            if (promotionNews != null)
            {
                result.Data = promotionNews;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении новости что-то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveNews(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemovePromotionNews(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Новость не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadNewsList()
        {
            var result = new JsonResultModel();
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var news = DataWrapper.GetPromotionNews(branchId);

            if (news != null)
            {
                result.Data = news;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки новостей что-то пошло не так";
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
                stock.Image = "../Images/default-image.jpg";
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
                result.ErrorMessage = "При добавлении акции что-то пошло не так...";
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
                result.ErrorMessage = "При загрузки акций что-то пошло не так";
            }

            return Json(result);
        }

        #region To do унифицировать логику
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
        public void UpdateOrderNumberConstructorProducts(List<UpdaterOrderNumber> data)
        {
            if (data != null && data.Any())
            {
                DataWrapper.UpdateOrderNumberConstructorProducts(data);
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

        #endregion

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
                result.ErrorMessage = "При загрузки отзывов что-то пошло не так";
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
                result.ErrorMessage = "При загрузки заказов, что-то пошло не так";
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
                result.ErrorMessage = "При загрузки заказов, что-то пошло не так";
            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public JsonResult LoadOrderItems(List<int> productIds, List<int> constructorCategoryIds)
        {
            var result = new JsonResultModel();

            if ((productIds == null || !productIds.Any())
                && (constructorCategoryIds == null || !constructorCategoryIds.Any()))
            {
                result.ErrorMessage = "Список идентификторов пуст";
            }
            else
            {
                var products = new List<ProductModel>();
                var ingredients = new Dictionary<int, List<IngredientModel>>();

                if (productIds != null)
                {
                    productIds = productIds.Distinct().ToList();
                    products = DataWrapper.GetOrderProducts(productIds);
                }

                if (constructorCategoryIds != null)
                {
                    ingredients = DataWrapper.GetIngredientsByCategoryIdVisible(constructorCategoryIds);
                }

                if (products == null && ingredients == null)
                {
                    result.ErrorMessage = "При загрузки продуктов что-то пошло не так";
                }
                else
                {
                    var detailConstructorProducts = new List<OrderDetailConstructorProduct>();
                    var detailProducts = new List<OrderDetailProduct>();

                    if (ingredients != null)
                    {
                        var categories = DataWrapper.GetCategories(ingredients.Keys);
                        detailConstructorProducts = ingredients.Select(p =>
                        {
                            var category = categories[p.Key];

                            return new OrderDetailConstructorProduct
                            {
                                CategoryId = category.Id,
                                CategoryName = category.Name,
                                CategoryImage = category.Image,
                                Ingredients = p.Value,
                            };

                        })
                        .ToList();
                    }

                    if (products != null)
                    {
                        var idsDict = products
                        .Select(p => p.CategoryId)
                        .Distinct()
                        .ToList();
                        var categoryDict = DataWrapper.GetCategories(idsDict);
                        detailProducts = products
                            .GroupBy(p => p.CategoryId)
                            .Select(p => new OrderDetailProduct
                            {
                                CategoryId = p.Key,
                                CategoryName = categoryDict[p.Key].Name,
                                Products = p.ToList()
                            })
                            .ToList();
                    }

                    result.Success = true;
                    result.Data = new
                    {
                        products = detailProducts,
                        constructor = detailConstructorProducts
                    };
                }




            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public void UpdateStatusOrder(UpdaterOrderStatus data)
        {
            try
            {
                if (data.Status == OrderStatus.Processing || data.OrderId < 1)
                {
                    return;
                }

                var order = DataWrapper.GetOrder(data.OrderId);
                if(order.OrderStatus != OrderStatus.Processing)
                {
                    return;
                }

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                var date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                data.DateUpdate = date;

                DataWrapper.UpdateStatusOrder(data);

                if (data.Status == OrderStatus.Processed)
                {
                    new PromotionLogic().ProcessingVirtualMoney(data.OrderId, branchId);
                }
                else if (data.Status == OrderStatus.Cancellation)
                {
                    new PromotionLogic().Refund(data.OrderId);
                }
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
        public JsonResult SavePromotionCashbackSetting(PromotionCashbackSetting setting)
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
                var sections = DataWrapper.GetPromotionSectionSettings(branchId);
                var setting = DataWrapper.GetPromotionSetting(branchId);

                result.Data = new PromotionSettingWrapper
                {
                    Sections = sections,
                    Setting = setting
                };

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
        public JsonResult SavePromotionSettings(PromotionSettingWrapper setting)
        {
            var result = new JsonResultModel();

            try
            {
                if (setting == null || !setting.Sections.Any() || setting.Setting == null)
                    throw new Exception("Пустая настрока");

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                setting.Sections.ForEach(p => p.BranchId = branchId);
                setting.Setting.BranchId = branchId;

                var newSectionSettings = DataWrapper.SavePromotionSectionSettings(setting.Sections);
                var newSettings = DataWrapper.SavePromotionSetting(setting.Setting);

                result.Data = new PromotionSettingWrapper
                {
                    Sections = newSectionSettings,
                    Setting = newSettings
                };
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
        public void RemoveCategoryConstructor(int categoryConstructorId)
        {
            try
            {
                DataWrapper.RemoveConstructorCategory(categoryConstructorId);
                DataWrapper.RemoveIngredientsByCategoryConstructorId(categoryConstructorId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddOrUpdateCategoryConstructor(ProductConstructorIngredientModel category)
        {
            var result = new JsonResultModel();

            if (category.Ingredients == null || !category.Ingredients.Any())
            {
                result.ErrorMessage = "Отсутсвтуют ингредиенты";

                return Json(result);
            }

            try
            {
                category.Ingredients.ForEach(p =>
                {
                    if (!System.IO.File.Exists(Server.MapPath(p.Image)))
                    {
                        p.Image = "/images/default-image.jpg";
                    }
                });

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);

                var constructorCategory = category.ConvertToConstructorCategory();
                constructorCategory.BranchId = branchId;
                constructorCategory = DataWrapper.AddOrUpdateConstructorCategory(constructorCategory);

                if (constructorCategory != null)
                {
                    category.Ingredients.UpdateIngredientSubAndCategoryId(constructorCategory.Id, constructorCategory.CategoryId);
                    var ingredients = DataWrapper.AddOrUpdateIngredients(category.Ingredients);

                    if (ingredients == null)
                    {
                        result.ErrorMessage = "При добавлении ингредиентов что-то пошло не так...";
                    }
                    else
                    {
                        result.Data = ConstructorProductHelper.GetProductConstructorIngredient(constructorCategory, ingredients);
                        result.Success = true;
                    }
                }
                else
                {
                    result.ErrorMessage = "При добавлении категории конструктора что-то пошло не так...";
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
        public JsonResult LoadProductConstructorList(int idCategory)
        {
            var result = new JsonResultModel();
            var constructorCategories = DataWrapper.GetConstructorCategoriesVisible(idCategory);

            if (constructorCategories != null)
            {
                result.Data = new List<int>();
                if (constructorCategories.Any())
                {

                    var ids = constructorCategories.Select(p => p.Id).ToList();
                    var dictIngredients = DataWrapper.GetIngredientsVisible(ids);
                    var ProductConstructorIngredients = new List<ProductConstructorIngredientModel>();

                    constructorCategories.ForEach(p =>
                    {
                        List<IngredientModel> ingredients = null;

                        if (dictIngredients.TryGetValue(p.Id, out ingredients) && ingredients != null && ingredients.Any())
                        {
                            var ProductConstructorIngredient = ConstructorProductHelper.GetProductConstructorIngredient(p, ingredients);
                            ProductConstructorIngredients.Add(ProductConstructorIngredient);
                        }

                    });

                    if (ProductConstructorIngredients != null && ProductConstructorIngredients.Any())
                    {
                        result.Data = ProductConstructorIngredients.OrderBy(p => p.OrderNumber).ToList();
                    }
                }

                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки конструктора что-то пошло не так";
            }

            return Json(result);
        }

        //To Do:Сделать опцинольной
        private static readonly int LIMIT_PUSH_MESSAGE_TODAY = 5;

        [HttpPost]
        [Authorize]
        public JsonResult PushNotification(PushNotification pushNotification)
        {
            var result = new JsonResultModel();
            var message = new FCMMessage(pushNotification);

            if (string.IsNullOrEmpty(message.Title))
            {
                result.ErrorMessage = "Отсутствует заголовок сообщения";
                return Json(result);
            }
            else if (string.IsNullOrEmpty(message.Body))
            {
                result.ErrorMessage = "Отсутствует тело сообщения";
                return Json(result);
            }

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                var date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                var countMessagesSentToday = DataWrapper.GetCountPushMessageByDate(branchId, date);

                if (countMessagesSentToday >= LIMIT_PUSH_MESSAGE_TODAY)
                {
                    result.ErrorMessage = "Превышен дневной лимит push уведомлений";
                    return Json(result);
                }

                var pushMessage = new PushMessageModel(message, branchId, date);
                var savedMessage = DataWrapper.SavePushMessage(pushMessage);

                if (savedMessage == null)
                    throw new Exception("Ошибка при сохранении PUSH сообщения");

                if (!string.IsNullOrEmpty(message.ImageUrl))
                {
                    message.ImageUrl = Request.Url.GetLeftPart(UriPartial.Authority) + message.ImageUrl.Substring(2);
                }

                Task.Run(() =>
                {
                    var fcmAuthKeyPath = Server.MapPath("/Resource/FCMAuthKey.json");
                    var tokens = DataWrapper.GetDeviceTokens(branchId);

                    if (tokens == null || !tokens.Any())
                        return;

                    var fcm = new FCMNotification(fcmAuthKeyPath, tokens);
                    fcm.SendMulticastMessage(message);
                });

                result.Success = true;
                result.Data = new
                {
                    limitPushMessageToday = LIMIT_PUSH_MESSAGE_TODAY,
                    countMessagesSentToday = countMessagesSentToday + 1
                };
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При отправке PUSH сообщения что-то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetPushNotificationLimits()
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                var date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                var countMessagesSentToday = DataWrapper.GetCountPushMessageByDate(branchId, date);


                result.Success = true;
                result.Data = new
                {
                    limitPushMessageToday = LIMIT_PUSH_MESSAGE_TODAY,
                    countMessagesSentToday
                };
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При получении лимитов push уведомлений что-то пошло не так...";
            }

            return Json(result);
        }

        private readonly int PAGE_PUSH_MESSAGE_SIZE = 10;

        [HttpPost]
        [Authorize]
        public JsonResult LoadPushNotification(int pageNumber)
        {
            var result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var messagesCount = DataWrapper.GetCountPushMessage(branchId);
                var messagesMaxPage = messagesCount == 0 ? 1 : Convert.ToInt32(Math.Ceiling((double)messagesCount / PAGE_PUSH_MESSAGE_SIZE));
                pageNumber = pageNumber < 1 ? 1 : pageNumber;

                var historyMessage = DataWrapper.GetPushMessage(branchId, pageNumber, PAGE_PUSH_MESSAGE_SIZE);

                result.Success = true;
                result.Data = new PagingPushMessageHistory
                {
                    HistoryMessages = historyMessage,
                    PageNumber = pageNumber,
                    PageSize = PAGE_PUSH_MESSAGE_SIZE,
                    IsLast = messagesMaxPage == pageNumber
                };
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При загрузге истории PUSH сообщений что-то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public void CheerupServer()
        {
            //reg.ru засыпает через 5 минут простоя
            //поэтому мы раз в 2 минуту сюда стучимся
            //что бы он не уснусл когда страница открыта
        }
    }
}