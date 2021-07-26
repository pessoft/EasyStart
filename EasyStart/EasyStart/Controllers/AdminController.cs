﻿using EasyStart.Logic;
using EasyStart.Logic.FCM;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.ConstructorProduct;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.GeneralSettings;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.ProductReview;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Models.ProductOption;
using EasyStart.Repository;
using EasyStart.Services;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace EasyStart.Controllers
{
    [RedirectingAction]
    [Authorize]
    public class AdminController : Controller
    {
        private JsonResultModel result;
        private DeliverySettingService deliverySettingService;
        private BranchService branchService;
        private OrderService orderService;
        private UtilsService utilsService;
        private GeneralSettingsService generalSettingsService;
        private CategoryProductService categoryProductService;
        private ProductService productService;
        private BranchRemovalService branchRemovalService;
        private PromotionService promotionService;
        private ConstructorProductService constructorProductService;
        private ProductReviewService productReviewService;
        private PushNotificationService pushNotificationService;

        public AdminController()
        { }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            var context = new AdminPanelContext();
            var repositoryFactory = new RepositoryFactory(context);
            var imageLogic = new ContainImageLogic();
            var displayItemSettingLogic = new DisplayItemSettingLogic();

            var orderLogic = new OrderLogic(repositoryFactory);
            var integrationSystemLogic = new IntegrationSystemLogic(repositoryFactory);
            var productLogic = new ProductLogic(
               repositoryFactory,
                imageLogic,
                displayItemSettingLogic);
            var deliverySettingLogic = new DeliverySettingLogic(repositoryFactory);
            var branchLogic = new BranchLogic(repositoryFactory, User.Identity.Name);
            var pushNotificationLogic = new PushNotificationLogic(repositoryFactory, branchLogic);
            var clientLogic = new ClientLogic(repositoryFactory);
            var generalSettingLogic = new GeneralSettingsLogic(repositoryFactory);
            var categoryProductLogic = new CategoryProductLogic(
                repositoryFactory,
                imageLogic,
                displayItemSettingLogic);
            var constructorProductLogic = new ConstructorProductLogic(
                repositoryFactory,
                displayItemSettingLogic,
                imageLogic);
            var promotionLogic = new Logic.Services.Promotion.PromotionLogic(
                repositoryFactory,
                imageLogic);
            var productReviewLogic = new ProductReviewLogic(repositoryFactory, displayItemSettingLogic);

            pushNotificationService = new PushNotificationService(pushNotificationLogic);
            orderService = new OrderService(
                orderLogic,
                integrationSystemLogic,
                productLogic,
                deliverySettingLogic,
                pushNotificationLogic);
            deliverySettingService = new DeliverySettingService(deliverySettingLogic, branchLogic);
            branchService = new BranchService(branchLogic, generalSettingLogic);
            utilsService = new UtilsService(new UtilsLogic());
            generalSettingsService = new GeneralSettingsService(generalSettingLogic, branchLogic);
            categoryProductService = new CategoryProductService(categoryProductLogic, productLogic, branchLogic);
            productService = new ProductService(productLogic, branchLogic, categoryProductLogic);
            branchRemovalService = new BranchRemovalService(
                branchLogic,
                generalSettingLogic,
                deliverySettingLogic,
                clientLogic,
                categoryProductLogic,
                productLogic,
                constructorProductLogic);
            promotionService = new PromotionService(promotionLogic, branchLogic, deliverySettingLogic);
            constructorProductService = new ConstructorProductService(constructorProductLogic, categoryProductLogic, branchLogic);
            productReviewService = new ProductReviewService(productReviewLogic);
        }

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

                var deliverySetting = deliverySettingService.GetByBranchId(branchId);

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
            try
            {
                var url = utilsService.SaveImage(Request);
                result = JsonResultModel.CreateSuccessWithDataURL(url);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке изображения что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveSetting(SettingModel setting)
        {
            try
            {
                var savedSetting = generalSettingsService.SaveSettings(setting);
                result = JsonResultModel.CreateSuccess(savedSetting);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении натсройки что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveDeliverySetting(DeliverySettingModel setting)
        {
            try
            {
                var savedSetting = deliverySettingService.SaveDeliverySetting(setting);
                result = JsonResultModel.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении натсройки что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddCategory(CategoryModel category)
        {
            return SaveCategory(category);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveCategory(CategoryModel category)
        {
            try
            {
                var savedCategory = categoryProductService.SaveCategory(category);
                result = JsonResultModel.CreateSuccess(savedCategory);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении категории что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddProduct(ProductModel product)
        {
            return SaveProduct(product);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddBranch(NewBranchModel newBranch)
        {
            try
            {
                var viewBranch = branchService.AddBranch(newBranch);
                result = JsonResultModel.CreateSuccess(viewBranch);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);

                if (ex is BranchActionPermissionDeniedException
                    || ex is BranchAlreadyExistException)
                    result = JsonResultModel.CreateError(ex.Message);
                else
                    result = JsonResultModel.CreateError("При добавлении филиала что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveBranch(int id)
        {
            try
            {
                branchRemovalService.Remove(id);
                result = JsonResultModel.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);

                if (ex is BranchActionPermissionDeniedException
                    || ex is BranchSelfDeletionException)
                    result = JsonResultModel.CreateError(ex.Message);
                else
                    result = JsonResultModel.CreateError("При удалении филиала что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadBranchList()
        {
            try
            {
                var branchViews = branchService.GetBranches();
                result = JsonResultModel.CreateSuccess(branchViews);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке филиалов что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadMainProductData()
        {
            try
            {
                var data = new
                {
                    Categories = categoryProductService.GetByBranch(),
                    AdditionalOptions = productService.GetAdditionalOptionsByBranch(),
                    AdditionalFillings = productService.GetAdditionalFillingsByBranch()
                };
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке категорий что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveCategory(int id)
        {
            try
            {
                var success = categoryProductService.RemoveCategory(id);

                if (success)
                    result = JsonResultModel.CreateSuccess(success);
                else
                    result = JsonResultModel.CreateError($"Категория с идентификатором {id} не найдена");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При удалении категории что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateCategory(CategoryModel category)
        {
            return SaveCategory(category);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadProductList(int idCategory)
        {
            try
            {
                var products = productService.GetByCategory(idCategory);

                result = JsonResultModel.CreateSuccess(products);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке продуктов что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveProduct(int id)
        {
            try
            {
                productService.RemoveProduct(id);

                result = JsonResultModel.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При удалении продуктов что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveProduct(ProductModel product)
        {
            try
            {
                var savedProduct = productService.SaveProduct(product);
                result = JsonResultModel.CreateSuccess(savedProduct);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении продукта что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateProduct(ProductModel product)
        {
            return SaveProduct(product);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveNews(PromotionNewsModel promotionNews)
        {
            try
            {
                var savedNews = promotionService.SaveNews(promotionNews);
                result = JsonResultModel.CreateSuccess(savedNews);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении новости что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveNews(int id)
        {
            try
            {
                promotionService.RemovePromotionNews(id);
                result = JsonResultModel.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При удалении новости что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadNewsList()
        {
            try
            {
                var news = promotionService.GetNews();
                result = JsonResultModel.CreateSuccess(news);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке новостей что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveStock(StockModel stock)
        {
            try
            {
                var savedStock = promotionService.SaveStock(stock);
                result = JsonResultModel.CreateSuccess(savedStock);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При добавлении акции что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveStock(int id)
        {
            try
            {
                promotionService.RemoveStock(id);
                result = JsonResultModel.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При удалении акции что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadStockList()
        {
            try
            {
                var stocks = promotionService.GetStocks();
                result = JsonResultModel.CreateSuccess(stocks);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке акции что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public void UpdateOrderNumberCategory(List<UpdaterOrderNumber> data)
        {
            try
            {
                categoryProductService.UpdateOrder(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateOrderNumberConstructorProducts(List<UpdaterOrderNumber> data)
        {
            try
            {
                constructorProductService.UpdateOrder(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateOrderNumberProducts(List<UpdaterOrderNumber> data)
        {
            try
            {
                productService.UpdateOrder(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateVisibleCategory(UpdaterVisible data)
        {
            try
            {
                categoryProductService.UpdateVisible(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public void UpdateVisibleProduct(UpdaterVisible data)
        {
            try
            {
                productService.UpdateVisible(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadProductReviews(int productId)
        {
            try
            {
                var reviews = productReviewService.Get(productId);
                result = JsonResultModel.CreateSuccess(reviews);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке отзывов что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public void UpdateVisibleReview(UpdaterVisible data)
        {
            try
            {
                productReviewService.UpdateVisible(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadOrders(List<int> branchIds)
        {
            try
            {
                var data = new
                {
                    Orders = orderService.GetByBranchIds(branchIds),
                    TodayData = orderService.GetDataOrdersByDate(branchIds, DateTime.Now)
                };
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке заказов что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadHistoryOrders(HistoryOrderFilter filter)
        {
            try
            {
                var orders = orderService.GetHistory(filter);
                result = JsonResultModel.CreateSuccess(orders);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке заказов что-то пошло не так...");
            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public JsonResult LoadOrderItems(List<int> productIds, List<int> categoryIds)
        {
            try
            {
                var data = new
                {
                    products = productService.GetProductOrderDetails(productIds),
                    constructor = constructorProductService.GetConstructorProductOrderDetails(categoryIds)
                };
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке продуктов из заказа что-то пошло не так...");
            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public void UpdateStatusOrder(UpdaterOrderStatus data)
        {
            try
            {
                orderService.ChangeOrderStatus(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetTodayOrderData(List<int> branchIds)
        {
            try
            {
                var data = orderService.GetDataOrdersByDate(branchIds, DateTime.Now);
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При информации о заказах что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadCoupons()
        {
            try
            {
                var coupons = promotionService.GetCoupons();
                result = JsonResultModel.CreateSuccess(coupons);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузки купонов что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveCoupon(CouponModel newCoupon)
        {
            try
            {
                var coupon = promotionService.SaveCoupon(newCoupon);
                result = JsonResultModel.CreateSuccess(coupon);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении купона что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveCoupon(int id)
        {
            try
            {
                promotionService.RemoveCoupon(id);
                result = JsonResultModel.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При удалении купона что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadCashbackPartnerSettings()
        {
            try
            {
                var data = new
                {
                    CashbackSetting = promotionService.GetPromotionCashbackSetting(),
                    PartnersSetting = promotionService.GetPromotionPartnerSetting()
                };
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке настроек кешбека что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SavePromotionCashbackSetting(PromotionCashbackSetting setting)
        {
            try
            {
                var data = promotionService.SavePromotionCashbackSetting(setting);
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении настроек кешбека что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SavePromotionPartnerSetting(PromotionPartnerSetting setting)
        {
            try
            {
                var data = promotionService.SavePromotionPartnerSetting(setting);
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении настроек партнерской программы что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadPromotionSettings()
        {
            try
            {
                var data = promotionService.GetPromotionGeneralSetting();
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При сохранении настроек продвижения что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SavePromotionSettings(PromotionGeneralSetting setting)
        {
            try
            {
                var data = promotionService.SavePromotionGeneralSettings(setting);
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загузке настроек продвижения что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public void RemoveCategoryConstructor(int categoryConstructorId)
        {
            try
            {
                constructorProductService.RemoveConstructorCategory(categoryConstructorId);
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
            try
            {
                var data = constructorProductService.AddOrUpdateCategoryConstructor(category);
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При добавлении категории конструктора что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadProductConstructorList(int categoryId)
        {
            try
            {
                var data = constructorProductService.GetConstructorProducts(categoryId);
                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузке конструктора что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult PushNotification(PushNotification pushNotification)
        {
            try
            {
                var uriDomain = Request.Url.GetLeftPart(UriPartial.Authority);
                var info = pushNotificationService.PushNotification(pushNotification, uriDomain);
                var data = new
                {
                    limitPushMessageToday = info.LimitPushMessageToday,
                    countMessagesSentToday = info.CountMessagesSentToday
                };

                result = JsonResultModel.CreateSuccess(data);
            }
            catch (PushNotifictionException ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При отправке PUSH сообщения что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetPushNotificationLimits()
        {
            try
            {
                var info = pushNotificationService.GetPushNotificationInfo();
                var data = new
                {
                    limitPushMessageToday = info.LimitPushMessageToday,
                    countMessagesSentToday = info.CountMessagesSentToday
                };

                result = JsonResultModel.CreateSuccess(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При получении лимитов PUSH уведомлений что-то пошло не так...");
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadPushNotification(int pageNumber)
        {
            result = new JsonResultModel();

            try
            {
                var data = pushNotificationService.GetPagingPushMessageHistory(pageNumber);
                result= JsonResultModel.CreateSuccess(data);

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result = JsonResultModel.CreateError("При загрузге истории PUSH сообщений что-то пошло не так...");
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

        [HttpPost]
        [Authorize]
        public JsonResult SaveProductAdditionalOption(AdditionalOption additionalOption)
        {
            result = new JsonResultModel();

            try
            {
                if (additionalOption == null
                    || additionalOption.Items == null
                    || !additionalOption.Items.Any())
                    throw new Exception("Пустные значение не допустимы");

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                additionalOption.BranchId = branchId;

                var savedAdditionalOption = DataWrapper.SaveProductAdditionalOption(additionalOption);

                if (savedAdditionalOption != null
                    && savedAdditionalOption.Items != null
                    && savedAdditionalOption.Items.Any())
                {
                    result.Success = true;
                    result.Data = savedAdditionalOption;
                }
                else
                    throw new Exception("Ошибка сохранения дополнительных опций");

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При сохранении дополнительных опций что-то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveProductAdditionalOption(int id)
        {
            result = new JsonResultModel();

            try
            {
                if (id < 1)
                    throw new Exception("Не корректный идентификатор");

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var succesRemove = DataWrapper.RemoveProductAdditionalOptionById(id);

                if (succesRemove)
                {
                    result.Success = true;
                }
                else
                    throw new Exception("Ошибка удаления дополнительных опций");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При удалении дополнительных опций что-то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveAdditionalFilling(AdditionalFilling additionalFilling)
        {
            result = new JsonResultModel();

            try
            {
                if (additionalFilling == null
                    || string.IsNullOrEmpty(additionalFilling.Name))
                    throw new Exception("Пустные значение не допустимы");

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                additionalFilling.BranchId = branchId;

                var savedAdditionalFilling = DataWrapper.SaveAdditionalFilling(additionalFilling);

                if (savedAdditionalFilling != null)
                {
                    result.Success = true;
                    result.Data = savedAdditionalFilling;
                }
                else
                    throw new Exception("Ошибка сохранения дополнительных опций");

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При сохранении дополнительных опций что-то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveAdditionalFilling(int id)
        {
            result = new JsonResultModel();

            try
            {
                if (id < 1)
                    throw new Exception("Не корректный идентификатор");

                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var succesRemove = DataWrapper.RemoveAdditionalFilling(id);

                if (succesRemove)
                {
                    result.Success = true;
                }
                else
                    throw new Exception("Ошибка удаления дополнительных опций");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При удалении дополнительных опций что-то пошло не так...";
            }

            return Json(result);
        }
    }
}