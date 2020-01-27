using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.Notification;
using EasyStart.Logic.Notification.EmailNotification;
using EasyStart.Logic.Transaction;
using EasyStart.Models;
using EasyStart.Models.Notification;
using EasyStart.Models.Transaction;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyStart
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminAppController : ApiController
    {
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

        [HttpPost]
        public JsonResultModel GetMainData([FromBody] MainDataSignatureModel data)
        {
            var result = new JsonResultModel();
            var branchId = data.BranchId;
            result.Success = false;

            if (branchId < 1 || data.ClientId < 1)
                return result;

            try
            {
                var categories = GetCategories(branchId);
                var products = GetAllProducts(branchId);
                var constructorCategories = GetConstructorCategories(branchId);
                var ingredients = GetIngredients(constructorCategories.Keys);
                var deliverySettings = DataWrapper.GetDeliverySetting(branchId);
                var organizationSettings = DataWrapper.GetSetting(branchId);

                var promotionLogic = new PromotionLogic();
                var stocks = promotionLogic.GetStockForAPI(branchId, data.ClientId);
                var mainBranch = DataWrapper.GetMainBranch();
                var promotionCashbackSetting = promotionLogic.GetSettingCashBack(mainBranch.Id);
                var promotionPartnersSetting = promotionLogic.GetSettingPartners(mainBranch.Id);
                var promotionSectionSettings = promotionLogic.GetSettingSections(branchId);
                var promotionSetting = DataWrapper.GetPromotionSetting(branchId);

                var productIds = products.Values.SelectMany(p => p.Select(s => s.Id)).ToList();
                var reviewsCount = DataWrapper.GetProductReviewsVisibleCount(productIds);

                result.Data = new
                {
                    categories,
                    products,
                    constructorCategories,
                    ingredients,
                    deliverySettings,
                    organizationSettings,
                    stocks,
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

        public List<CategoryModel> GetCategories(int branchId)
        {
            try
            {
                var categories = DataWrapper.GetCategoriesVisible(branchId);
                categories.ForEach(p => PreprocessorDataAPI.ChangeImagePath(p));

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
        public JsonResultModel SendOrder([FromBody]OrderModel order)
        {
            var result = new JsonResultModel();

            try
            {
                if (order == null)
                    throw new Exception("Попытка оформления пустого заказа");

                var deliverSetting = DataWrapper.GetDeliverySetting(order.BranchId);
                var client = DataWrapper.GetClient(order.ClientId);

                order.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                order.UpdateDate = order.Date;

                if (order.AmountPayCashBack > 0
                    && (client.VirtualMoney - order.AmountPayCashBack) < 0)
                {
                    throw new Exception("Не достаточно виртуальных средств");
                }

                var numberOrder = DataWrapper.SaveOrder(order);

                if (numberOrder != -1)
                {
                    if (order.AmountPayCashBack > 0)
                    {
                        client.VirtualMoney -= order.AmountPayCashBack;

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
                    result.Data = numberOrder;
                    result.Success = true;

                    var currentContext = System.Web.HttpContext.Current;
                    Task.Run(() => Notification(currentContext, order, deliverSetting));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        private void Notification(System.Web.HttpContext currentContext, OrderModel order, DeliverySettingModel deliverSetting)
        {
            try
            {
                System.Web.HttpContext.Current = currentContext;
                var emailTemplate = File.ReadAllText(currentContext.Server.MapPath("~/Resource/EmailTemplate.html"));
                var setting = DataWrapper.GetSetting(order.BranchId);
                var categoryConstructor = DataWrapper.GetCategories(order.BranchId).Where(p => p.CategoryType == CategoryType.Constructor).ToList();
                var constructorIngredients = order.ProductConstructorCount != null ?
                DataWrapper.GetIngredients(order.ProductConstructorCount.SelectMany(p => p.IngrdientCount.Keys)) :
                null;
                var products = DataWrapper.GetOrderProducts(order.ProductCount.Keys.ToList());
                var bonusProducts = order.ProductBonusCount != null ?
                    DataWrapper.GetOrderProducts(order.ProductBonusCount.Keys.ToList()):
                    new List<ProductModel>();
                var orderInfoParams = new OrderInfoParams
                {
                    Setting = setting,
                    Products = products,
                    BonusProducts = bonusProducts,
                    CategoryConstructor = categoryConstructor,
                    ConstructorIngredients = constructorIngredients
                };
                var optionsNotification = new OptionsNotificationNewOrderModel
                {
                    DomainUr = Request.RequestUri.GetBaseUrl(),
                    Email = new Email(),
                    EmailBodyHTMLTemplate = emailTemplate,
                    Order = order,
                    OrderInfo = order.GetOrderInfo(orderInfoParams),
                    ToEmail = string.IsNullOrEmpty(deliverSetting.NotificationEmail) ? null : new List<string> { deliverSetting.NotificationEmail }
                };

                new NotifyNewOrderManager(optionsNotification).AllNotify();
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        public JsonResultModel GetHistoryOrders([FromBody]DataHistoryForViewModel dataHistoryForLoad)
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
        public JsonResultModel GetProductsHistoryOrder([FromBody]int orderId)
        {
            var result = new JsonResultModel();

            try
            {
                var history = DataWrapper.GetOrder(orderId);
                var productsHistory = new List<ProductHistoryModel>();
                var constructorProductsHistory = new List<ConstructorProductHistoryModel>();

                if (history.ProductCount != null && history.ProductCount.Any())
                {
                    var products = DataWrapper.GetProducts(history.ProductCount.Keys.ToList());

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
                            var ingredients = DataWrapper.GetIngredients(p.IngrdientCount.Keys);
                            var price = ingredients.Sum(x => x.Price * p.IngrdientCount[x.Id]);
                            var ingredientsHistory = new List<IngredientHistoryModel>();
                            var isDeleted = category.IsDeleted || ingredients.Exists(x => x.IsDeleted);

                            ingredients.ForEach(x =>
                            {
                                ingredientsHistory.Add(new IngredientHistoryModel
                                {
                                    Id = x.Id,
                                    CategoryId = x.CategoryId,
                                    SubCategoryId = x.SubCategoryId,
                                    Count = p.IngrdientCount[x.Id],
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

                
                result.Data = new { productsHistory, constructorProductsHistory };
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
        public JsonResultModel UpdateProductRating([FromBody]RatingProductUpdater ratingUp)
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
        public void SetProductReviews([FromBody]ProductReview review)
        {
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
                    DataWrapper.SaveProductReviews(review);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        [HttpPost]
        public JsonResultModel GetProductReviews([FromBody]int productId)
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
        public JsonResultModel AddOrUpdateClient([FromBody]Client client)
        {
            var result = new JsonResultModel();
            try
            {
                if (string.IsNullOrEmpty(client.PhoneNumber) ||
                    string.IsNullOrEmpty(client.UserName))
                    return result;

                var oldClient = DataWrapper.GetClientByPhoneNumber(client.PhoneNumber);

                var saveTransaction = false;

                if (oldClient == null)
                {
                    client.ReferralCode = KeyGenerator.GetUniqueKey(8);
                }


                Action setDefaultReferralData = () =>
                {
                    client.ParentReferralClientId = 0;
                    client.ParentReferralCode = null;
                };

                if (oldClient != null && oldClient.ParentReferralClientId < 1
                    && !string.IsNullOrEmpty(client.ParentReferralCode)
                    && oldClient.ReferralCode != client.ParentReferralCode
                    || oldClient == null && !string.IsNullOrEmpty(client.ParentReferralCode))
                {
                    var parentClient = DataWrapper.GetClientByByReferralCode(client.ParentReferralCode);

                    if (parentClient != null)
                    {
                        client.ParentReferralClientId = parentClient.Id;
                        client.ParentReferralCode = parentClient.ReferralCode;

                        var mainBranch = DataWrapper.GetMainBranch();
                        var partnersSetting = DataWrapper.GetPromotionPartnerSetting(mainBranch.Id);

                        if (partnersSetting.IsUsePartners)
                        {
                            switch (partnersSetting.TypeBonusValue)
                            {
                                case DiscountType.Ruble:
                                    client.VirtualMoney = partnersSetting.BonusValue;
                                    saveTransaction = true;
                                    break;
                                case DiscountType.Percent:
                                    client.ReferralDiscount = partnersSetting.BonusValue;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        setDefaultReferralData();
                    }
                }
                else if (oldClient != null)
                {
                    client.ParentReferralClientId = oldClient.ParentReferralClientId;
                    client.ParentReferralCode = oldClient.ParentReferralCode;
                    client.ReferralDiscount = oldClient.ReferralDiscount;
                }
                else
                {
                    setDefaultReferralData();
                }

                var newClient = DataWrapper.AddOrUpdateClient(client);

                if (saveTransaction)
                {
                    var transactionLogic = new TransactionLogic();
                    transactionLogic.AddPartnersTransaction(PartnersTransactionType.EnrollmentReferralBonus, newClient.Id, newClient.VirtualMoney);
                }

                result.Data = new
                {
                    clientId = newClient.Id,
                    phoneNumber = newClient.PhoneNumber,
                    userName = newClient.UserName,
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
                return result;
            }
        }

        [HttpPost]
        public JsonResultModel CheckActualUserData([FromBody]UserDataPhoneApp userData)
        {
            var result = new JsonResultModel
            {
                Success = true,
                Data = false
            };

            try
            {
                var setting = DataWrapper.GetSettingByCity(userData.CityId);
                var client = DataWrapper.GetClient(userData.ClientId);
                var isPhoneEquals = client != null ? client.PhoneNumber == userData.PhoneNumber : false;
                if (setting != null &&
                    client != null &&
                    isPhoneEquals)
                {
                    result.Data = new
                    {
                        isLogin = true,
                        clientId = client.Id,
                        phoneNumber = client.PhoneNumber,
                        userName = client.UserName,
                        referralCode = client.ReferralCode,
                        parentReferralClientId = client.ParentReferralClientId,
                        parentReferralCode = client.ParentReferralCode,
                        virtualMoney = client.VirtualMoney,
                        referralDiscount = client.ReferralDiscount,
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                return result;
            }
        }


        [HttpPost]
        public JsonResultModel GetCoupun([FromBody]CouponParamsModel data)
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
        public JsonResultModel GetPartnersTransaction([FromBody]int clientId)
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

                result.ErrorMessage = "При загрузке партнерских транзакций пошло что то не так";
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel GetCashbackTransaction([FromBody]int clientId)
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

                result.ErrorMessage = "При загрузке транзакций кешбека пошло что то не так";
            }

            return result;
        }
    }
}