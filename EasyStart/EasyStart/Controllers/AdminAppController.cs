using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.Notification;
using EasyStart.Logic.Notification.EmailNotification;
using EasyStart.Logic.Transaction;
using EasyStart.Models;
using EasyStart.Models.Notification;
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
                var deliverySettings = DataWrapper.GetDeliverySetting(branchId);
                var organizationSettings = DataWrapper.GetSetting(branchId);

                var promotionLogic = new PromotionLogic();
                var stocks = promotionLogic.GetStockForAPI(branchId, data.ClientId);
                var coupons = promotionLogic.GetCoupons(branchId);
                var mainBranch = DataWrapper.GetMainBranch();
                var promotionCashbackSetting = promotionLogic.GetSettingCashBack(mainBranch.Id);
                var promotionPartnersSetting = promotionLogic.GetSettingPartners(mainBranch.Id);
                var promotionSectionSettings = promotionLogic.GetSettingSections(mainBranch.Id);

                var productIds = products.Values.SelectMany(p => p.Select(s => s.Id)).ToList();
                var reviewsCount = DataWrapper.GetProductReviewsVisibleCount(productIds);

                result.Data = new
                {
                    categories,
                    products,
                    deliverySettings,
                    organizationSettings,
                    stocks,
                    promotionCashbackSetting,
                    promotionPartnersSetting,
                    promotionSectionSettings,
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
                .ToDictionary(p => p.Key, p => p.ToList());

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
                    Task.Run(() =>
                    {
                        System.Web.HttpContext.Current = currentContext;
                        var emailTemplate = File.ReadAllText(currentContext.Server.MapPath("~/Resource/EmailTemplate.html"));
                        var setting = DataWrapper.GetSetting(order.BranchId);
                        var products = DataWrapper.GetOrderProducts(order.ProductCount.Keys.ToList());
                        var optionsNotification = new OptionsNotificationNewOrderModel
                        {
                            DomainUr = Request.RequestUri.GetBaseUrl(),
                            Email = new Email(),
                            EmailBodyHTMLTemplate = emailTemplate,
                            Order = order,
                            OrderInfo = order.GetOrderInfo(setting, products),
                            ToEmail = string.IsNullOrEmpty(deliverSetting.NotificationEmail) ? null : new List<string> { deliverSetting.NotificationEmail }
                        };

                        new NotifyNewOrderManager(optionsNotification).AllNotify();
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        [HttpPost]
        public JsonResultModel GetHistoryOrder([FromBody]DataHistoryForViewModel dataHistoryForLoad)
        {
            var result = new JsonResultModel();

            try
            {
                var historyOrder = DataWrapper.GetHistoryOrder(dataHistoryForLoad.ClientId, dataHistoryForLoad.BranchId);

                result.Data = historyOrder;
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

                var saveTransaction = false;
                if (client.Id < 1)
                {
                    client.ReferralCode = KeyGenerator.GetUniqueKey(8);

                    if (client.ParentReferralClientId > 0)
                    {
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
                coupon = DataWrapper.GetCouponByPromocode(data.BranchId, data.Promocode);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            result.Data = coupon;

            return result;
        }
    }
}