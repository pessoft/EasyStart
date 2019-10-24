using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.Notification;
using EasyStart.Logic.Notification.EmailNotification;
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
            result.Success = false;

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
            { }

            return branchCityDict;
        }

        [HttpPost]
        public JsonResultModel GetMainData([FromBody] int branchId)
        {
            var result = new JsonResultModel();
            result.Success = false;

            if (branchId < 1)
                return result;

            try
            {
                var categories = GetCategories(branchId);
                var products = GetAllProducts(branchId);
                var deliverySettings = DataWrapper.GetDeliverySetting(branchId);
                var organizationSettings = DataWrapper.GetSetting(branchId);
                var stocks = DataWrapper.GetStocks(branchId);

                var productIds = products.Values.SelectMany(p => p.Select(s => s.Id)).ToList();
                var reviewsCount = DataWrapper.GetProductReviewsVisibleCount(productIds);

                foreach (var id in productIds)
                {
                    var outCount = 0;

                    if (!reviewsCount.TryGetValue(id, out outCount))
                    {
                        reviewsCount.Add(id, 0);
                    }
                }

                //TO DO: вынести в метод
                categories.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.Image))
                    {
                        p.Image = p.Image.Substring(2);
                    }
                });

                stocks.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.Image))
                    {
                        p.Image = p.Image.Substring(2);
                    }
                });

                foreach (var kv in products)
                {
                    kv.Value.ForEach(p =>
                    {
                        if (!string.IsNullOrEmpty(p.Image))
                        {
                            p.Image = p.Image.Substring(2);
                        }
                    });
                }

                result.Data = new
                {
                    categories,
                    products,
                    deliverySettings,
                    organizationSettings,
                    stocks,
                    reviewsCount
                };
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
            }

            return result;
        }

        public List<CategoryModel> GetCategories(int branchId)
        {
            try
            {
                var categories = DataWrapper.GetCategoriesVisible(branchId);

                return categories;
            }
            catch (Exception ex)
            {
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

                return products;
            }
            catch (Exception ex)
            {
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

                order.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                order.UpdateDate = order.Date;

                var numberOrder = DataWrapper.SaveOrder(order);

                if (numberOrder != -1)
                {
                    order.Id = numberOrder;
                    result.Data = numberOrder;
                    result.Success = true;

                    var server = System.Web.HttpContext.Current.Server;
                    Task.Run(() =>
                    {
                        var emailTemplate = File.ReadAllText(server.MapPath("~/Resource/EmailTemplate.html"));
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

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
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
                return result;
            }

        }

        [HttpPost]
        public JsonResultModel UpdateProducRating([FromBody]RatingProductUpdater ratingUp)
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
            { }

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
            { }
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
            { }

            return result;
        }

        public List<StockModel> GetStocks(int cityId)
        {
            try
            {
                var branchId = DataWrapper.GetBranchIdByCity(cityId);
                var stocks = DataWrapper.GetStocksVisible(branchId);

                return stocks;
            }
            catch (Exception ex)
            {
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

                var newClient = DataWrapper.AddOrUpdateClient(client);

                result.Data = new
                {
                    clientId = newClient.Id,
                    phoneNumber = newClient.PhoneNumber,
                    userName = newClient.UserName
                };
                result.Success = true;

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        [HttpPost]
        public JsonResultModel CheckActualUserData([FromBody]UserDataPhoneApp userData)
        {
            var result = new JsonResultModel();
            result.Success = true;
            result.Data = false;

            try
            {
                var setting = DataWrapper.GetSettingByCity(userData.CityId);
                var client = DataWrapper.GetClient(userData.ClientId);
                var isPhoneEquals = client != null ? client.PhoneNumber == userData.PhoneNumber : false;
                if (setting != null &&
                    client != null &&
                    isPhoneEquals)
                {
                    result.Data = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
    }
}