using EasyStart.HtmlRenderer;
using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Notification;
using EasyStart.Logic.Notification.EmailNotification;
using EasyStart.Logic.OrderProcessor;
using EasyStart.Logic.Transaction;
using EasyStart.Migrations;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Models.Notification;
using EasyStart.Models.OnlinePay;
using EasyStart.Models.OnlinePay.Fondy;
using EasyStart.Models.ProductOption;
using EasyStart.Models.Transaction;
using EasyStart.Repositories;
using EasyStart.Services;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace EasyStart
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminAppController : ApiController
    {
        private readonly ClientService clientService;
        private readonly IntegrationSystemService integrationSystemService;
        private readonly BranchService branchService;
        private readonly OrderService orderService;
        private readonly IOrderProcesser orderProcesser;

        public AdminAppController()
        {
            var context = new AdminPanelContext();

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            integrationSystemService = new IntegrationSystemService(inegrationSystemRepository);

            var clientRepository = new ClientRepository(context);
            clientService = new ClientService(clientRepository);

            var branchRepository = new BranchRepository(context);
            branchService = new BranchService(branchRepository);

            var orderRepository = new OrderRepository(context);
            orderService = new OrderService(orderRepository);

            orderProcesser = new OrderProcessor();
        }

        public JsonResultModel GetLocation()
        {
            var result = new JsonResultModel();

            try
            {
                var allowedCity = GetAllowedCity();
                var cityBranches = GetCityBranches();

                result.Data = new { cities = allowedCity, cityBranches };
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                Logger.Log.Error(ex);
            }

            return result;
        }

        public Dictionary<int, string> GetAllowedCity()
        {
            try
            {
                var alloweCityIds = DataWrapper.GetAllowedCity();
                var cityDictionary = CityHelper.City
                    .Where(p => alloweCityIds.Exists(s => s == p.Key))
                    .ToDictionary(p => p.Key, p => p.Value);

                return cityDictionary;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public Dictionary<int, int> GetCityBranches()
        {
            var branchCityDict = new Dictionary<int, int>();

            try
            {
                var alloweCityIds = DataWrapper.GetAllowedCity();

                foreach (var cityId in alloweCityIds)
                {
                    var branchId = DataWrapper.GetBranchIdByCity(cityId);
                    branchCityDict.Add(cityId, branchId);
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return branchCityDict;
        }

        private DeliverySettingModel GetDeliverySettingForAPI(int branchId)
        {
            var deliverySettings = DataWrapper.GetDeliverySetting(branchId);
            deliverySettings.PaymentKey = "";
            deliverySettings.CreditKey = "";

            return deliverySettings;
        }

        [HttpPost]
        public JsonResultModel GetMainData([FromBody] MainDataSignatureModel data)
        {
            var result = new JsonResultModel();

            try
            {
                var appPackageName = ConfigurationManager.AppSettings["AppPackageName"];
                if (!string.IsNullOrEmpty(appPackageName))
                {
                    if (appPackageName != data.AppPackageName)
                        throw new Exception($"Имя пакета приложения не совпадает. Excepted: {appPackageName}, actual: {data.AppPackageName ?? "null"}");
                }

                var branchId = data.BranchId;
                var clientid = data.ClientId;
                result.Success = false;

                if (branchId < 1 || data.ClientId < 1)
                {
                    var mainBranch = DataWrapper.GetMainBranch();
                    branchId = mainBranch.Id;
                    clientid = -1;
                }

                var categories = GetCategories(branchId);
                var products = GetAllProducts(branchId);
                var additionalOptions = GetAdditionalOptions(branchId);
                var additionalFillings = GetAdditionalFillings(branchId);
                var constructorCategories = GetConstructorCategories(branchId);
                var ingredients = GetIngredients(constructorCategories.Keys);
                var deliverySettings = GetDeliverySettingForAPI(branchId);
                var organizationSettings = DataWrapper.GetSetting(branchId);

                var promotionLogic = new PromotionLogic();
                var stocks = promotionLogic.GetStockForAPI(branchId, clientid);
                var news = promotionLogic.GetNewsForAPI(branchId);
                var promotionCashbackSetting = promotionLogic.GetSettingCashBack(branchId);
                var promotionPartnersSetting = promotionLogic.GetSettingPartners(branchId);
                var promotionSectionSettings = promotionLogic.GetSettingSections(branchId);
                var promotionSetting = DataWrapper.GetPromotionSetting(branchId);

                var productIds = products.Values.SelectMany(p => p.Select(s => s.Id)).ToList();
                var reviewsCount = DataWrapper.GetProductReviewsVisibleCount(productIds);

                var recommendedProducts = DataWrapper.GetRecommendedProductsForCategoryByBranchId(branchId);

                result.Data = new
                {
                    categories,
                    products,
                    recommendedProducts,
                    additionalOptions,
                    additionalFillings,
                    constructorCategories,
                    ingredients,
                    deliverySettings,
                    organizationSettings,
                    stocks,
                    news,
                    promotionCashbackSetting,
                    promotionPartnersSetting,
                    promotionSectionSettings,
                    promotionSetting
                };
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.Success = false;
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel GetStocks([FromBody] MainDataSignatureModel data)
        {
            var result = new JsonResultModel();
            var branchId = data.BranchId;
            var clientid = data.ClientId;
            result.Success = false;

            if (branchId < 1 || data.ClientId < 1)
            {
                result.Data = new { stocks = new List<int>() };
                result.Success = true;

                return result;
            }

            try
            {
                var promotionLogic = new PromotionLogic();
                var stocks = promotionLogic.GetStockForAPI(branchId, clientid);

                result.Data = new { stocks };
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.Success = false;
            }

            return result;
        }

        public List<CategoryModel> GetCategories(int branchId)
        {
            try
            {
                var categories = DataWrapper.GetCategoriesVisible(branchId);
                for (var i = 0; i < categories.Count; ++i)
                {
                    var category = categories[i];
                    category.OrderNumber = i + 1;
                    PreprocessorDataAPI.ChangeImagePath(category);
                }

                return categories;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public List<ProductModel> GetProducts(int categoryId)
        {
            try
            {
                var products = DataWrapper.GetProducts(categoryId);

                return products;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public Dictionary<int, List<ProductModel>> GetAllProducts(int branchId)
        {
            try
            {
                var products = DataWrapper.GetAllProductsVisible(branchId)
                .GroupBy(p => p.CategoryId)
                .ToDictionary(
                    p => p.Key,
                    p => p.OrderBy(x => x.OrderNumber).ToList());

                foreach (var kv in products)
                {
                    kv.Value.ForEach(p => PreprocessorDataAPI.ChangeImagePath(p));
                }

                return products;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public Dictionary<int, AdditionalOption> GetAdditionalOptions(int branchId)
        {
            var additionalOptions = DataWrapper.GetAllProductAdditionalOptionByBranchId(branchId);
            additionalOptions = additionalOptions ?? new List<AdditionalOption>();

            return additionalOptions.ToDictionary(p => p.Id);
            //var additionalFillings = DataWrapper.GetAllAdditionalFillingsByBranchId(branchId);
            //additionalFillings = additionalFillings ?? new List<AdditionalFilling>();
        }

        public Dictionary<int, Models.ProductOption.AdditionalFilling> GetAdditionalFillings(int branchId)
        {
            var additionalFillings = DataWrapper.GetAllAdditionalFillingsByBranchId(branchId);
            additionalFillings = additionalFillings ?? new List<Models.ProductOption.AdditionalFilling>();

            return additionalFillings.ToDictionary(p => p.Id);
        }

        /// <summary>
        /// key - category id
        /// </summary>
        /// <param name="branckId"></param>
        /// <returns></returns>
        public Dictionary<int, List<ConstructorCategory>> GetConstructorCategories(int branckId)
        {
            try
            {
                var categories = DataWrapper.GetConstuctorCategoriesByBranchId(branckId);
                var result = new Dictionary<int, List<ConstructorCategory>>();

                if (categories != null)
                {
                    result = categories.GroupBy(p => p.CategoryId).ToDictionary(p => p.Key, p => p.ToList());
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public Dictionary<int, List<IngredientModel>> GetIngredients(IEnumerable<int> categoryIds)
        {
            try
            {
                var ingredients = DataWrapper.GetAllDictionaryIngredientsByCategoryIds(categoryIds);

                foreach (var kv in ingredients)
                {
                    kv.Value.ForEach(p => PreprocessorDataAPI.ChangeImagePath(p));
                }

                return ingredients;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public DeliverySettingModel GetDeliverySetting(int cityId)
        {
            try
            {
                var deliverySetting = DataWrapper.GetDeliverySettingByCity(cityId);

                return deliverySetting;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        public SettingModel GetSetting(int cityId)
        {
            try
            {
                var setting = DataWrapper.GetSettingByCity(cityId);

                return setting;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public JsonResultModel CompleteOrderPay([FromBody] int orderId)
        {
            var result = new JsonResultModel();
            OrderModel order = null;

            try
            {
                var currentContext = System.Web.HttpContext.Current;
                order = DataWrapper.GetOrder(orderId);

                if (order == null)
                    throw new Exception($"Заказ {orderId} не найден");

                var deliverSetting = DataWrapper.GetDeliverySetting(order.BranchId);

                Func<string> getSignature = () =>
                {
                    var currency = "RUB";
                    var amount = Utils.Utils.ConvertRubToKopeks(order.AmountPayDiscountDelivery);
                    var signatureStr = $"{deliverSetting.PaymentKey}|{deliverSetting.MerchantId}|{order.Id}";
                    var signatureSHA = Utils.Utils.SHA1(signatureStr);
                    return signatureSHA;
                };

                Func<Task<PaymentStatus>> checkPaymentStatus = async () =>
               {
                   string json = new JavaScriptSerializer().Serialize(new
                   {
                       request = new
                       {
                           order_id = order.Id,
                           merchant_id = deliverSetting.MerchantId,
                           signature = getSignature()
                       }
                   });

                   var rContent = "";
                   using (var client = new HttpClient())
                   {
                       var response = client.PostAsync(
                           "https://api.fondy.eu/api/status/order_id",
                            new StringContent(json, Encoding.UTF8, "application/json")).Result;
                       rContent = await response.Content.ReadAsStringAsync();
                   }

                   var indexOpenBracket = rContent.LastIndexOf("{");
                   var length = rContent.IndexOf("}") - indexOpenBracket + 1;
                   var jsonResponse = rContent.Substring(indexOpenBracket, length);
                   var payStatus = JsonConvert.DeserializeObject<PaymentStatus>(jsonResponse);

                   return payStatus;
               };

                var paymentStatus = checkPaymentStatus().Result;

                if (paymentStatus.ResponseStatus == StatusResponse.SUCCESS)
                {
                    if (order.OrderStatus == OrderStatus.PendingPay)
                    {
                        var date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                        var dateUpdate = date;
                        var approximateDeliveryTime = order.DateDelivery.HasValue ?
                            order.DateDelivery :
                            order.Date + orderProcesser.GetAverageOrderProcessingTime(order.BranchId, order.DeliveryType);
                        var updateOrderStatus = new UpdaterOrderStatus
                        {
                            DateUpdate = dateUpdate,
                            ApproximateDeliveryTime = approximateDeliveryTime,
                            OrderId = order.Id,
                            Status = OrderStatus.Processing
                        };

                        DataWrapper.UpdateStatusOrder(updateOrderStatus);
                        var updateOrder = DataWrapper.GetOrder(order.Id);

                        if (updateOrder.OrderStatus == OrderStatus.Processing)
                        {
                            result.Success = true;
                            result.Data = new
                            {
                                deliveryType = order.DeliveryType,
                                orderNumber = orderId,
                                approximateDeliveryTime = approximateDeliveryTime.HasValue ?
                                    TimeZoneInfo.ConvertTimeToUtc(approximateDeliveryTime.Value, TimeZoneInfo.FindSystemTimeZoneById(deliverSetting.ZoneId)) :
                                    approximateDeliveryTime
                            };
                            Task.Run(() =>
                            {
                                updateOrder = SendOrderToIntegrationSystem(currentContext, updateOrder);
                                NotifyAboutNewOrder(currentContext, updateOrder, deliverSetting);

                            });
                        }
                        else
                            throw new Exception("При обработке заказа что-то пошло не так");
                    }
                    else
                    {
                        result.ErrorMessage = "Закак уже был оплачен";
                    }
                }
                else
                {
                    result.ErrorMessage = FondyErrorCodeHelper.GetErrorDescription(paymentStatus.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.Success = false;
                result.ErrorMessage = ex.Message;

                if (order != null)
                    CancelOrder(order.Id, order.BranchId);
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel SendOrder([FromBody] OrderModel order)
        {
            var result = new JsonResultModel();

            try
            {
                if (order == null)
                    throw new Exception("Попытка оформления пустого заказа");

                var client = DataWrapper.GetClient(order.ClientId);

                // to do fix in mobile app
                #region hot fix 
                if (client.BranchId < 1)
                {
                    var branchId = DataWrapper.GetBranchIdByCity(client.CityId);
                    client.BranchId = branchId;
                    client = DataWrapper.UpdateClient(client);
                }
                #endregion
                if (order.BranchId != client.BranchId)
                {
                    order.BranchId = client.BranchId;
                }

                if (order.OrderStatus != OrderStatus.PendingPay)
                {
                    order.OrderStatus = OrderStatus.Processing;
                }

                var deliverSetting = DataWrapper.GetDeliverySetting(order.BranchId);

                if (order.DateDelivery != null)
                    order.DateDelivery = order.DateDelivery.Value.GetDateTimeNow(deliverSetting.ZoneId);

                order.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                order.UpdateDate = order.Date;
                order.ApproximateDeliveryTime = order.DateDelivery.HasValue ?
                            order.DateDelivery :
                            order.Date + orderProcesser.GetAverageOrderProcessingTime(order.BranchId, order.DeliveryType);

                if (order.AmountPayCashBack > 0
                    && (client.VirtualMoney - order.AmountPayCashBack) < 0)
                {
                    throw new Exception("Не достаточно виртуальных средств");
                }

                order.AmountPayCashBack = Math.Round(order.AmountPayCashBack, 2);
                order.AmountPayDiscountDelivery = Math.Round(order.AmountPayDiscountDelivery, 2);
                var numberOrder = DataWrapper.SaveOrder(order);

                if (numberOrder != -1)
                {
                    if (order.StockIds != null)
                    {
                        var isUseStockWithTriggerBirthday = DataWrapper.IsUseStockWithTriggerBirthday(order.StockIds, order.BranchId);

                        if (isUseStockWithTriggerBirthday)
                        {
                            DataWrapper.FixUseStockWithBirthday(order.ClientId, order.Date);
                        }
                    }


                    if (order.AmountPayCashBack > 0)
                    {
                        client.VirtualMoney -= order.AmountPayCashBack;
                        client.VirtualMoney = Math.Round(client.VirtualMoney, 2);
                        DataWrapper.ClientUpdateVirtualMoney(client.Id, client.VirtualMoney);

                        var transactionLogic = new TransactionLogic();
                        transactionLogic.AddCashbackTransaction(CashbackTransactionType.OrderPayment, client.Id, numberOrder, order.AmountPayCashBack);

                    }

                    if (order.ReferralDiscount > 0)
                    {
                        DataWrapper.ClientUpdateReferralDiscount(order.ClientId, 0);
                    }

                    if (order.CouponId > 0)
                    {
                        new PromotionLogic().UseCopun(order.CouponId);
                    }

                    order.Id = numberOrder;
                    result.Data = new
                    {
                        deliveryType = order.DeliveryType,
                        orderNumber = numberOrder,
                        approximateDeliveryTime = order.ApproximateDeliveryTime.HasValue ?
                           TimeZoneInfo.ConvertTimeToUtc(order.ApproximateDeliveryTime.Value, TimeZoneInfo.FindSystemTimeZoneById(deliverSetting.ZoneId)) :
                            order.ApproximateDeliveryTime
                    };
                    result.Success = true;

                    if (order.OrderStatus == OrderStatus.Processing)
                    {
                        var currentContext = System.Web.HttpContext.Current;
                        Task.Run(() =>
                        {
                            order = SendOrderToIntegrationSystem(currentContext, order);
                            NotifyAboutNewOrder(currentContext, order, deliverSetting);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                CancelOrder(order.Id, order.BranchId);
            }

            return result;
        }

        private void CancelOrder(int orderId, int branchId)
        {
            try
            {
                if (orderId < 1)
                    return;

                if (branchId < 1)
                    branchId = DataWrapper.GetMainBranch().Id;

                var data = new UpdaterOrderStatus { OrderId = orderId, Status = OrderStatus.Deleted };
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

        private OrderModel SendOrderToIntegrationSystem(System.Web.HttpContext currentContext, OrderModel order)
        {
            System.Web.HttpContext.Current = currentContext;
            var integrationSetting = integrationSystemService.Get(order.BranchId);
            OrderModel updatedOrder = order;

            if (integrationSetting.UseAutomaticDispatch)
            {
                Thread.Sleep(1500);// немного притормаживаем из-за ограничений фронтпада максимум 2 запроса в 1 секунду
                var result = orderProcesser.SendOrderToIntegrationSystem(order.Id);

                if (result.Success)
                {
                    updatedOrder = orderService.Get(order.Id);
                }
            }

            return updatedOrder;

        }

        private void NotifyAboutNewOrder(System.Web.HttpContext currentContext, OrderModel order, DeliverySettingModel deliverSetting)
        {
            try
            {
                var useState = UseMethod.GetCurrentState();
                if (!useState)
                    return;

                System.Web.HttpContext.Current = currentContext;
                var emailTemplate = File.ReadAllText(currentContext.Server.MapPath("~/Resource/EmailTemplateOrderNotify.html"));
                var setting = DataWrapper.GetSetting(order.BranchId);
                var categoryConstructor = DataWrapper.GetCategories(order.BranchId).Where(p => p.CategoryType == CategoryType.Constructor).ToList();
                var constructorIngredients = order.ProductConstructorCount != null ?
                DataWrapper.GetIngredients(order.ProductConstructorCount.SelectMany(p => p.IngredientCount.Keys)) :
                null;
                var products = order.ProductCount != null ?
                    DataWrapper.GetOrderProducts(order.ProductCount.Keys) :
                    new List<ProductModel>();
                var bonusProducts = order.ProductBonusCount != null ?
                    DataWrapper.GetOrderProducts(order.ProductBonusCount.Keys) :
                    new List<ProductModel>();
                var productsWithOptions = order.ProductWithOptionsCount != null ?
                    DataWrapper.GetOrderProducts(order.ProductWithOptionsCount.Select(p => p.ProductId)) :
                    new List<ProductModel>();
                var bonusProductsWithOptions = bonusProducts.Where(p => p.ProductAdditionalOptionIds.Count > 0).ToList();
                bonusProducts.RemoveAll(p => p.ProductAdditionalOptionIds.Count > 0);


                var additionalOptions = DataWrapper.GetAllProductAdditionalOptionByBranchId(order.BranchId)?.ToDictionary(p => p.Id) ?? new Dictionary<int, AdditionalOption>();
                var additionalFillings = DataWrapper.GetAllAdditionalFillingsByBranchId(order.BranchId)?.ToDictionary(p => p.Id) ?? new Dictionary<int, Models.ProductOption.AdditionalFilling>();

                var orderInfoParams = new OrderInfoParams
                {
                    Setting = setting,
                    Products = products,
                    BonusProducts = bonusProducts,
                    ProductsWithOptions = productsWithOptions,
                    BonusProductsWithOptions = bonusProductsWithOptions,
                    CategoryConstructor = categoryConstructor,
                    ConstructorIngredients = constructorIngredients,
                    AdditionalOptions = additionalOptions,
                    AdditionalFillings = additionalFillings,
                };
                var optionsNotification = new OptionsNotificationNewOrderModel
                {
                    DomainUr = Request.RequestUri.GetBaseUrl(),
                    EmailSender = new EmailSender(),
                    EmailBodyHTMLTemplate = emailTemplate,
                    Order = order,
                    OrderInfo = order.GetOrderInfo(orderInfoParams),
                    ToEmail = string.IsNullOrEmpty(deliverSetting.NotificationEmail) ? null : new List<string> { deliverSetting.NotificationEmail }
                };

                new NotifyNewOrderManager(optionsNotification).AllNotify();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }


        [HttpPost]
        public JsonResultModel GetHistoryOrders([FromBody] DataHistoryForViewModel dataHistoryForLoad)
        {
            var result = new JsonResultModel();

            try
            {
                var historyOrders = DataWrapper.GetHistoryOrders(dataHistoryForLoad.ClientId, dataHistoryForLoad.BranchId);

                result.Data = historyOrders;
                result.Success = true;

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return result;
            }
        }

        [HttpPost]
        public JsonResultModel GetProductsHistoryOrder([FromBody] int orderId)
        {
            var result = new JsonResultModel();

            try
            {
                var history = DataWrapper.GetOrder(orderId);
                var productsHistory = new List<ProductHistoryModel>();
                var constructorProductsHistory = new List<ConstructorProductHistoryModel>();
                var productsWithOptionsHistory = new List<ProductWithOptionsHistoryModel>();

                if (history.ProductCount != null && history.ProductCount.Any())
                {
                    var products = DataWrapper.GetProducts(history.ProductCount.Keys);

                    if (products != null)
                    {
                        products.ForEach(p =>
                        {
                            var productHistory = new ProductHistoryModel
                            {
                                Id = p.Id,
                                CategoryId = p.CategoryId,
                                AdditionInfo = p.AdditionInfo,
                                CategoryType = CategoryType.Default,
                                Count = history.ProductCount[p.Id],
                                Image = p.Image,
                                IsDeleted = p.IsDeleted,
                                Name = p.Name,
                                Price = p.Price
                            };
                            PreprocessorDataAPI.ChangeImagePath(productHistory);
                            productsHistory.Add(productHistory);
                        });
                    }
                }

                if (history.ProductConstructorCount != null && history.ProductConstructorCount.Any())
                {
                    var categories = DataWrapper.GetCategories(history.ProductConstructorCount.Select(p => p.CategoryId));

                    history.ProductConstructorCount.ForEach(p =>
                    {
                        CategoryModel category = null;

                        if (categories.TryGetValue(p.CategoryId, out category))
                        {
                            var ingredients = DataWrapper.GetIngredients(p.IngredientCount.Keys);
                            var price = ingredients.Sum(x => x.Price * p.IngredientCount[x.Id]);
                            var ingredientsHistory = new List<IngredientHistoryModel>();
                            var isDeleted = category.IsDeleted || ingredients.Exists(x => x.IsDeleted);

                            ingredients.ForEach(x =>
                            {
                                ingredientsHistory.Add(new IngredientHistoryModel
                                {
                                    Id = x.Id,
                                    CategoryId = x.CategoryId,
                                    SubCategoryId = x.SubCategoryId,
                                    Count = p.IngredientCount[x.Id],
                                    Name = x.Name,
                                    Price = x.Price
                                });
                            });

                            var constructorProductHistory = new ConstructorProductHistoryModel
                            {
                                Id = p.CategoryId,
                                CategoryType = CategoryType.Constructor,
                                Count = p.Count,
                                Image = category.Image,
                                Name = category.Name,
                                Price = price,
                                Ingredients = ingredientsHistory,
                                IsDeleted = isDeleted

                            };
                            PreprocessorDataAPI.ChangeImagePath(constructorProductHistory);
                            constructorProductsHistory.Add(constructorProductHistory);
                        }
                    });
                }

                if (history.ProductWithOptionsCount != null && history.ProductWithOptionsCount.Any())
                {
                    var products = DataWrapper.GetProductDictionary(history.ProductWithOptionsCount.Select(p => p.ProductId));

                    if (products != null)
                    {
                        Func<ProductWithOptionsOrderModel,
                            ProductModel,
                        Tuple<double,
                        Dictionary<int, AdditionalOption>,
                        Dictionary<int, Models.ProductOption.AdditionalFilling>>> getOptionsData = (pWoptions, product) =>
                        {
                            var additionalOptionsIds = pWoptions.AdditionalOptions?.Keys.ToList() ?? new List<int>();
                            var additionalOptionsItemIds = pWoptions.AdditionalOptions?.Values.ToList() ?? new List<int>();
                            var additionalOptions = DataWrapper.GetProductAdditionalOptionsByIds(additionalOptionsIds, additionalOptionsItemIds)
                            ?? new Dictionary<int, AdditionalOption>();

                            var additionalFillingIds = pWoptions.AdditionalFillings ?? new List<int>();
                            var additionalFillings = DataWrapper.GetAdditionalFillingsByIds(additionalFillingIds)
                            ?? new Dictionary<int, Models.ProductOption.AdditionalFilling>();

                            var priceWithOptions = product.Price;

                            foreach (var additionalOption in additionalOptions.Values)
                            {
                                if (additionalOption.Items != null)
                                    priceWithOptions += additionalOption.Items.Sum(p => p.Price);
                            }

                            foreach (var additionalFilling in additionalFillings.Values)
                            {
                                priceWithOptions += additionalFilling.Price;
                            }

                            priceWithOptions *= pWoptions.Count;

                            return Tuple.Create(priceWithOptions, additionalOptions, additionalFillings);
                        };

                        history.ProductWithOptionsCount.ForEach(p =>
                        {
                            if (products.TryGetValue(p.ProductId, out ProductModel product))
                            {
                                var optionsData = getOptionsData(p, product);
                                var productHistory = new ProductWithOptionsHistoryModel
                                {
                                    Id = product.Id,
                                    CategoryId = product.CategoryId,
                                    AdditionInfo = product.AdditionInfo,
                                    CategoryType = CategoryType.WithOptions,
                                    Count = p.Count,
                                    Image = product.Image,
                                    IsDeleted = product.IsDeleted,
                                    Name = product.Name,
                                    Price = product.Price,
                                    PriceWithOptions = optionsData.Item1,
                                    AdditionalOptions = optionsData.Item2,
                                    AdditionalFillings = optionsData.Item3
                                };
                                PreprocessorDataAPI.ChangeImagePath(productHistory);
                                productsWithOptionsHistory.Add(productHistory);
                            }
                        });
                    }
                }

                result.Data = new { productsHistory, constructorProductsHistory, productsWithOptionsHistory };
                result.Success = true;

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return result;
            }
        }

        [HttpPost]
        public JsonResultModel UpdateProductRating([FromBody] RatingProductUpdater ratingUp)
        {
            var result = new JsonResultModel();

            try
            {
                ratingUp.Score = ratingUp.Score > 0 ? ratingUp.Score : 0;

                DataWrapper.SaveRating(new RatingProduct
                {
                    ClientId = ratingUp.ClientId,
                    ProductId = ratingUp.ProductId,
                    Score = ratingUp.Score
                });
                var rating = DataWrapper.GetProductRating(ratingUp.ProductId);

                var product = DataWrapper.GetProduct(ratingUp.ProductId);
                DataWrapper.UpdateRating(ratingUp.ProductId, rating.Rating, rating.VotesCount, rating.VotesSum);

                result.Success = true;
                result.Data = new
                {
                    product.CategoryId,
                    ProductId = product.Id,
                    rating.Rating,
                    rating.VotesCount,
                    rating.VotesSum
                };
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel SetProductReviews([FromBody] ProductReview review)
        {
            var result = new JsonResultModel();

            try
            {
                if (review != null &&
                   review.ClientId > 0 &&
                   !string.IsNullOrEmpty(review.ReviewText) &&
                   review.ProductId > 0)
                {
                    var branchId = DataWrapper.GetBranchIdByCity(review.CityId);
                    var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                    review.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                    var client = DataWrapper.GetClient(review.ClientId);
                    review.Reviewer = client.UserName;
                    var savedReview = DataWrapper.SaveProductReviews(review);

                    if (savedReview != null)
                    {
                        result.Success = true;
                        result.Data = savedReview;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel GetProductReviews([FromBody] int productId)
        {
            var result = new JsonResultModel();
            try
            {
                var reviews = DataWrapper.GetProductReviewsVisible(productId);

                if (reviews != null && reviews.Any())
                {
                    reviews.ForEach(p =>
                    {
                        var hideNumberPhone = new StringBuilder(p.PhoneNumber);
                        hideNumberPhone[11] = '*';
                        hideNumberPhone[12] = '*';
                        hideNumberPhone[14] = '*';

                        p.PhoneNumber = hideNumberPhone.ToString();
                        p.Reviewer += " " + p.PhoneNumber;
                    });

                    result.Data = reviews;
                    result.Success = true;
                }
                else
                {
                    result.Data = new List<ProductReview>();
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public List<StockModel> GetStocks(int cityId)
        {
            try
            {
                var branchId = DataWrapper.GetBranchIdByCity(cityId);
                var stocks = DataWrapper.GetStocks(branchId);

                return stocks;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public JsonResultModel RegistrationClient([FromBody] Client client)
        {
            var result = new JsonResultModel();

            try
            {
                if (string.IsNullOrEmpty(client.PhoneNumber) ||
                   string.IsNullOrEmpty(client.Password))
                {
                    result.ErrorMessage = "Укажите регистрационные данные";
                    return result;
                }

                var isNewClient = DataWrapper.GetClientByPhoneNumber(client.PhoneNumber) == null;

                if (!isNewClient)
                {
                    result.ErrorMessage = "Номер телефона уже зарегистрирован";
                    return result;
                }

                client.ReferralCode = KeyGenerator.GetUniqueKey(8);
                var newClient = DataWrapper.RegistrationClient(client);

                var branchId = client.BranchId == 0 ? branchService.GetMainBranch().Id : client.BranchId;
                var virtualMoney = integrationSystemService.GetClientVirtualMoney(client.PhoneNumber, branchId, new IntegrationSystemFactory());
                clientService.SetVirtualMoney(newClient.Id, virtualMoney);

                result.Data = new
                {
                    isLogin = true,
                    clientId = newClient.Id,
                    phoneNumber = newClient.PhoneNumber,
                    password = newClient.Password,
                    email = newClient.Email,
                    userName = newClient.UserName,
                    dateBirth = newClient.DateBirth,
                    cityId = newClient.CityId,
                    branchId = newClient.BranchId,
                    referralCode = newClient.ReferralCode,
                    parentReferralClientId = newClient.ParentReferralClientId,
                    parentReferralCode = newClient.ParentReferralCode,
                    virtualMoney = virtualMoney,
                    referralDiscount = newClient.ReferralDiscount,
                };
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "Что-то пошло не так";
            }


            return result;
        }

        [HttpPost]
        public JsonResultModel UpdateParrentReferral([FromBody] ClientParentReferralCode clientParentReferralCode)
        {
            var result = new JsonResultModel();
            try
            {
                Client client = null;

                if (string.IsNullOrEmpty(clientParentReferralCode.PhoneNumber) ||
                    string.IsNullOrEmpty(clientParentReferralCode.Password) ||
                    (client = DataWrapper.GetClient(clientParentReferralCode.PhoneNumber, clientParentReferralCode.Password)) == null)
                {
                    result.ErrorMessage = "Не удалось авторизоваться";
                    return result;
                }

                if (client.ParentReferralClientId > 0)
                {
                    result.ErrorMessage = "Реферальный код уже установлен";
                    return result;
                }

                var saveTransaction = false;
                var parentClient = DataWrapper.GetClientByByReferralCode(clientParentReferralCode.ParentReferralCode);

                if (parentClient == null)
                {
                    result.ErrorMessage = "Неверный реферальный код";
                    return result;
                }

                var partnersSetting = DataWrapper.GetPromotionPartnerSetting(client.BranchId);
                if (parentClient != null && parentClient.ParentReferralClientId != client.Id &&
                    partnersSetting != null && partnersSetting.IsUsePartners)
                {
                    client.ParentReferralClientId = parentClient.Id;
                    client.ParentReferralCode = parentClient.ReferralCode;

                    switch (partnersSetting.TypeBonusValue)
                    {
                        case DiscountType.Ruble:
                            client.VirtualMoney += partnersSetting.BonusValue;
                            client.VirtualMoney = Math.Round(client.VirtualMoney, 2);
                            saveTransaction = true;
                            break;
                        case DiscountType.Percent:
                            client.ReferralDiscount = partnersSetting.BonusValue;
                            break;
                    }
                }
                else
                {
                    result.ErrorMessage = "Реферальный код не установлен";
                    return result;
                }

                var newClient = DataWrapper.UpdateClient(client);

                if (saveTransaction)
                {
                    var transactionLogic = new TransactionLogic();
                    transactionLogic.AddPartnersTransaction(PartnersTransactionType.EnrollmentReferralBonus, newClient.Id, newClient.VirtualMoney);
                }

                result.Data = new
                {
                    parentReferralClientId = newClient.ParentReferralClientId,
                    parentReferralCode = newClient.ParentReferralCode,
                    virtualMoney = newClient.VirtualMoney,
                    referralDiscount = newClient.ReferralDiscount,
                };
                result.Success = true;

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "Что-то пошло не так";
                return result;
            }
        }

        [HttpPost]
        public JsonResultModel UpdateClient([FromBody] Client client)
        {
            var result = new JsonResultModel();
            try
            {
                Client oldClient = null;

                if (string.IsNullOrEmpty(client.PhoneNumber) ||
                    string.IsNullOrEmpty(client.Password) ||
                    (oldClient = DataWrapper.GetClient(client.PhoneNumber, client.Password)) == null)
                {
                    result.ErrorMessage = "Не удалось авторизоваться";
                    return result;
                }

                if (!DataWrapper.ValidatEmail(client.PhoneNumber, client.Email))
                {
                    result.ErrorMessage = "E-mail уже зарегистрирован";
                    return result;
                }

                client.ParentReferralClientId = oldClient.ParentReferralClientId;
                client.ParentReferralCode = oldClient.ParentReferralCode;
                client.ReferralDiscount = oldClient.ReferralDiscount;
                client.VirtualMoney = oldClient.VirtualMoney;

                var branchId = DataWrapper.GetBranchIdByCity(client.CityId);
                client.BranchId = branchId;
                var newClient = DataWrapper.UpdateClient(client);

                result.Data = new
                {
                    clientId = newClient.Id,
                    phoneNumber = newClient.PhoneNumber,
                    password = newClient.Password,
                    email = newClient.Email,
                    userName = newClient.UserName,
                    dateBirth = newClient.DateBirth,
                    cityId = newClient.CityId,
                    branchId = newClient.BranchId,
                    referralCode = newClient.ReferralCode,
                    parentReferralClientId = newClient.ParentReferralClientId,
                    parentReferralCode = newClient.ParentReferralCode,
                    virtualMoney = newClient.VirtualMoney,
                    referralDiscount = newClient.ReferralDiscount,
                };
                result.Success = true;

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "Что-то пошло не так";
                return result;
            }
        }

        private void NotifyRestorePassword(System.Web.HttpContext currentContext, Client client)
        {
            try
            {
                var emailTemplate = File.ReadAllText(currentContext.Server.MapPath("~/Resource/EmailTemplateRestorePassword.html"));

                var htmpBodyRenderer = new EmailRestorePasswordBodyHtmlRenderer(client, emailTemplate);
                new RestorePasswordNotification(client, new EmailSender()).EmailNotify(htmpBodyRenderer);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        public JsonResultModel RestorePasswordClient([FromBody] string email)
        {
            var result = new JsonResultModel();
            try
            {
                var client = DataWrapper.GetClientByEmail(email);

                if (client == null)
                {
                    result.ErrorMessage = "Email не зарегистрирован";
                }
                else
                {
                    var currentContext = System.Web.HttpContext.Current;

                    Task.Run(() => NotifyRestorePassword(currentContext, client));

                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "Что-то пошло не так";
            }


            return result;
        }

        [HttpPost]
        public JsonResultModel Login([FromBody] UserDataPhoneApp userData)
        {
            var result = new JsonResultModel();

            try
            {

                if (string.IsNullOrEmpty(userData.PhoneNumber) ||
                  string.IsNullOrEmpty(userData.Password))
                {
                    result.ErrorMessage = "Укажите данные для входа";
                    return result;
                }

                var isClientExist = DataWrapper.GetClientByPhoneNumber(userData.PhoneNumber) != null;

                if (!isClientExist)
                {
                    result.ErrorMessage = "Номер телефона не зарегистрирован";
                    return result;
                }

                Client client = DataWrapper.GetClient(userData.PhoneNumber, userData.Password);

                if (client == null)
                {
                    result.ErrorMessage = "Неверный пароль";
                    return result;
                }
                else if (client.Blocked)
                {
                    result.ErrorMessage = "Учетная запись заблокирована";
                    return result;
                }

                var branchId = client.BranchId == 0 ? branchService.GetMainBranch().Id : client.BranchId;

                try
                {
                    Thread.Sleep(1500);
                    var virtualMoney = integrationSystemService.GetClientVirtualMoney(
                        client.PhoneNumber,
                        branchId, new IntegrationSystemFactory(),
                        client.VirtualMoney);
                    clientService.SetVirtualMoney(client.Id, virtualMoney);
                    client.VirtualMoney = virtualMoney;
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
                
                result.Success = true;
                result.Data = new
                {
                    isLogin = true,
                    clientId = client.Id,
                    phoneNumber = client.PhoneNumber,
                    password = client.Password,
                    email = client.Email,
                    userName = client.UserName,
                    dateBirth = client.DateBirth,
                    cityId = client.CityId,
                    branchId = client.BranchId,
                    referralCode = client.ReferralCode,
                    parentReferralClientId = client.ParentReferralClientId,
                    parentReferralCode = client.ParentReferralCode,
                    virtualMoney = client.VirtualMoney,
                    referralDiscount = client.ReferralDiscount,
                };

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return result;
            }
        }

        [HttpPost]
        public JsonResultModel GetCoupun([FromBody] CouponParamsModel data)
        {
            var result = new JsonResultModel { Success = true };
            CouponModel coupon = null;

            try
            {
                coupon = DataWrapper.GetCouponByPromocode(data);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            result.Data = coupon;

            return result;
        }

        [HttpPost]
        public JsonResultModel GetPartnersTransaction([FromBody] int clientId)
        {
            var result = new JsonResultModel();
            List<PartnersTransactionView> transactions = null;

            try
            {
                transactions = TransactionWrapper.GetPartnersTransactions(clientId);

                if (transactions != null)
                {
                    result.Success = true;
                    result.Data = transactions;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);

                result.ErrorMessage = "При загрузке партнерских транзакций что-то пошло не так";
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel GetCashbackTransaction([FromBody] int clientId)
        {
            var result = new JsonResultModel();
            List<CashbackTransaction> transactions = null;

            try
            {
                transactions = TransactionWrapper.GetCashbackTransactions(clientId);

                if (transactions != null)
                {
                    result.Success = true;
                    result.Data = transactions;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);

                result.ErrorMessage = "При загрузке транзакций кешбека что-то пошло не так";
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel RegisterDevice([FromBody] FCMDeviceModel device)
        {
            var result = new JsonResultModel();

            if (device == null ||
                string.IsNullOrEmpty(device.Token) ||
                device.ClientId < 1 ||
                device.BranchId < 1)
            {
                result.ErrorMessage = "Девайс не зарегестрирован";
                return result;
            }

            try
            {
                DataWrapper.AddOrUpdateDevice(device);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При регистрации девайса что-то пошло не так";
            }

            return result;
        }
    }
}