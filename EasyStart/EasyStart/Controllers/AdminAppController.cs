using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Models;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyStart
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminAppController : ApiController
    {
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
            try
            {
                var result = new JsonResultModel();
                var deliverSetting = DataWrapper.GetDeliverySetting(order.BranchId);

                order.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);
                order.UpdateDate = order.Date;

                var numberOrder = DataWrapper.SaveOrder(order);

                if (numberOrder != -1)
                {
                    order.Id = numberOrder;
                    new NewOrderHub().AddedNewOrder(order);

                    result.Data = numberOrder;
                    result.Success = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<OrderModel> GetHistoryOrder(int clientId)
        {
            try
            {
                var historyOrder = DataWrapper.GetHistoryOrder(clientId);

                return historyOrder;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [HttpPost]
        public void UpdateProducRating([FromBody]RatingProducUpdater ratingUp)
        {
            try
            {
                var result = new JsonResultModel();
                var product = DataWrapper.GetProduct(ratingUp.ProductId);
                var votesCount = ++product.VotesCount;
                var score = ratingUp.Score > 0 ? ratingUp.Score : 0;
                var votesSum = product.VotesSum + score;
                var rating = votesSum / votesCount;

                DataWrapper.UpdateRating(ratingUp.ProductId, rating, votesCount, votesSum);
            }
            catch (Exception ex)
            { }
        }

        [HttpPost]
        public void SetProductReviews([FromBody]ProductReview review)
        {
            try
            {
                if (review != null &&
               !string.IsNullOrEmpty(review.PhoneNumber) &&
               !string.IsNullOrEmpty(review.ReviewText) &&
               review.ProductId > 0)
                {
                    var branchId = DataWrapper.GetBranchIdByCity(review.CityId);
                    var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
                    review.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

                    DataWrapper.SaveProductReviews(review);
                }
            }
            catch (Exception ex)
            { }
        }

        public List<ProductReview> GetProductReviews(int productId)
        {
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

                        p.PhoneNumber = hideNumberPhone.ToString();
                    });
                }

                return reviews;
            }
            catch (Exception ex)
            {
                return null;
            }
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
        public Client AddOrUpdateClient([FromBody]Client client)
        {
            try
            {
                var newClient = DataWrapper.AddOrUpdateClient(client);

                return newClient;
            }
            catch (Exception ex)
            {
                return null;
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
                if (setting !=null &&
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