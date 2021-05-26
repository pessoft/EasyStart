using EasyStart.Logic;
using EasyStart.Logic.FCM;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.ConstructorProduct;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.GeneralSettings;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
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

        public AdminController()
        {}

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            var context = new AdminPanelContext();

            var orderRepository = new OrderRepository(context);
            var orderLogic = new OrderLogic(orderRepository);

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            var integrationSystemLogic = new IntegrationSystemLogic(inegrationSystemRepository);

            var productRepository = new ProductRepository(context);
            var additionalFillingRepository = new AdditionalFillingRepository(context);
            var additionOptionItemRepository = new AdditionOptionItemRepository(context);
            var productAdditionalFillingRepository = new ProductAdditionalFillingRepository(context);
            var productAdditionOptionItemRepository = new ProductAdditionOptionItemRepository(context);
            var additionalOptionRepository = new AdditionalOptionRepository(context);
            var productLogic = new ProductLogic(
                productRepository,
                additionalFillingRepository,
                additionalOptionRepository,
                additionOptionItemRepository,
                productAdditionalFillingRepository,
                productAdditionOptionItemRepository);

            var deliverySettingRepository = new DeliverySettingRepository(context);
            var areaDeliverySettingRepository = new AreaDeliveryRepository(context);
            var deliverySettingLogic = new DeliverySettingLogic(deliverySettingRepository, areaDeliverySettingRepository);

            var fcmDeviveRepository = new FCMDeviceRepository(context);
            var pushNotificationLogic = new PushNotificationLogic(fcmDeviveRepository);

            var branchRepository = new BranchRepository(context);
            var branchLogic = new BranchLogic(branchRepository, User.Identity.Name);

            var clientRepository = new ClientRepository(context);
            var clientLogic = new ClientLogic(clientRepository);

            var generalSettingRepository = new GeneralSettingRepository(context);
            var generalSettingLogic = new GeneralSettingsLogic(generalSettingRepository);

            var categoryProductRepository = new CategoryProductRepository(context);
            var recommendedProductRepository = new RecommendedProductRepository(context);
            var categoryProductLogic = new CategoryProductLogic(
                categoryProductRepository,
                recommendedProductRepository);

            var constructorCategoryRepository = new ConstructorCategoryRepository(context);
            var constructorIngredientRepository = new ConstructorIngredientRepository(context);
            var constructorProductLogic = new ConstructorProductLogic(
                constructorCategoryRepository,
                constructorIngredientRepository);

            var promotionNewsRepository = new PromotionNewsRepository(context);
            var promotionLogic = new Logic.Services.Promotion.PromotionLogic(promotionNewsRepository);

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
            productService = new ProductService(productLogic, branchLogic);
            branchRemovalService = new BranchRemovalService(
                branchLogic,
                generalSettingLogic,
                deliverySettingLogic,
                clientLogic,
                categoryProductLogic,
                productLogic,
                constructorProductLogic);
            promotionService = new PromotionService(promotionLogic, branchLogic);
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
                result = JsonResultModel.CreateError("При загрузке изображения что-то пошло не так");
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
                var viewBranch =  branchService.AddBranch(newBranch);
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
            result = new JsonResultModel();

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
            result = new JsonResultModel();
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
            result = new JsonResultModel();
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var stock = DataWrapper.GetStocks(branchId);

            if (stock != null)
            {
                result.Data = stock;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузке акций что-то пошло не так";
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
            result = new JsonResultModel();
            var reviews = DataWrapper.GetProductReviews(productId);

            if (reviews != null)
            {
                result.Data = reviews;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузке отзывов что-то пошло не так";
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
            result = new JsonResultModel();
            var orders = DataWrapper.GetOrders(brnachIds);
            var todayData = DataWrapper.GetDataOrdersByDate(brnachIds, DateTime.Now);

            if (orders != null)
            {
                result.Data = new { Orders = orders, TodayData = todayData };
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузке заказов, что-то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadHistoryOrders(HistoryOrderFilter filter)
        {
            result = new JsonResultModel();
            var orders = DataWrapper.GetHistoryOrders(filter.BranchIds, filter.StartDate, filter.EndDate);

            if (orders != null)
            {
                result.Data = orders;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузке заказов, что-то пошло не так";
            }

            return Json(result);
        }


        [HttpPost]
        [Authorize]
        public JsonResult LoadOrderItems(List<int> productIds, List<int> constructorCategoryIds)
        {
            result = new JsonResultModel();

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
                    result.ErrorMessage = "При загрузке продуктов что-то пошло не так";
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
                orderService.ChangeOrderStatus(data);
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
            result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = deliverySettingService.GetByBranchId(branchId);
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
            result = new JsonResultModel();

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
            result = new JsonResultModel();

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
            result = new JsonResultModel();

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
            result = new JsonResultModel();

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
            result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                setting.BranchId = branchId;

                var deliverSetting = deliverySettingService.GetByBranchId(branchId);
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
            result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                setting.BranchId = branchId;

                var deliverSetting = deliverySettingService.GetByBranchId(branchId);
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
            result = new JsonResultModel();

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
            result = new JsonResultModel();

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
            result = new JsonResultModel();

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
            result = new JsonResultModel();
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
                result.ErrorMessage = "При загрузке конструктора что-то пошло не так";
            }

            return Json(result);
        }

        //To Do:Сделать опцинольной
        private static readonly int LIMIT_PUSH_MESSAGE_TODAY = 5;

        [HttpPost]
        [Authorize]
        public JsonResult PushNotification(PushNotification pushNotification)
        {
            result = new JsonResultModel();
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
                var deliverSetting = deliverySettingService.GetByBranchId(branchId);
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
            result = new JsonResultModel();

            try
            {
                var branchId = DataWrapper.GetBranchId(User.Identity.Name);
                var deliverSetting = deliverySettingService.GetByBranchId(branchId);
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
            result = new JsonResultModel();

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
                result.ErrorMessage = "При сохранении дополнительных опций что-то пошло не так";
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
                result.ErrorMessage = "При удалении дополнительных опций что-то пошло не так";
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
                result.ErrorMessage = "При сохранении дополнительных опций что-то пошло не так";
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
                result.ErrorMessage = "При удалении дополнительных опций что-то пошло не так";
            }

            return Json(result);
        }
    }
}