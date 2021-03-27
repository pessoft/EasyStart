using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public class FrontPad : BaseIntegrationSystem
    {
        private const string newOrderUrl = "https://app.frontpad.ru/api/index.php?new_order";
        private const string getClientUrl = "https://app.frontpad.ru/api/index.php?get_client";

        private readonly FrontPadOptions frontPadOptions;
        public FrontPad(IntegrationSystemModel integrationSystemSetting): base(integrationSystemSetting)
        {
            frontPadOptions = JsonConvert.DeserializeObject<FrontPadOptions>(integrationSystemSetting.Options);
        }

        public override IntegrationOrderStatus GetIntegrationOrderStatus(int externalOrderStatusId)
        {
            var status = IntegrationOrderStatus.Unknown;

            if (externalOrderStatusId == this.frontPadOptions.StatusNew)
                status = IntegrationOrderStatus.New;
            else if (externalOrderStatusId == this.frontPadOptions.StatusProcessed)
                status = IntegrationOrderStatus.Preparing;
            else if (externalOrderStatusId == this.frontPadOptions.StatusDelivery)
                status = IntegrationOrderStatus.Deliverid;
            else if (externalOrderStatusId == this.frontPadOptions.StatusCancel)
                status = IntegrationOrderStatus.Canceled;
            else if (externalOrderStatusId == this.frontPadOptions.StatusDone)
                status = IntegrationOrderStatus.Done;

            return status;
        }

        public override INewOrderResult SendOrder(IOrderDetails orderDetails)
        {
            var postData = new StringBuilder();

            PrepareSecret(postData);
            PrepareProductData(orderDetails, postData);
            PrepareOrderData(orderDetails, postData);
            PrepareSalePointData(orderDetails, postData);
            PrepareHookUrlData(postData);

            var result = SendOrder(postData.ToString());

            return result;
        }

        public override double GetClinetVirtualMoney(string phoneNumber)
        {
            var postData = new StringBuilder();

            PrepareSecret(postData);
            PreparePhoneNumber(phoneNumber, postData, true);

            string responseResult = base.Post(getClientUrl, postData.ToString()).Result;
            var frontpadClientVirtualMoney = JsonConvert.DeserializeAnonymousType(responseResult, new { Score = 0.0 });

            return frontpadClientVirtualMoney.Score;
        }

        private INewOrderResult SendOrder(string postData)
        {
            string responseResult = base.Post(newOrderUrl, postData).Result;
            var frontPadResult = JsonConvert.DeserializeObject<FrontPadNewOrderResult>(responseResult);
            var result = new NewOrderResult(frontPadResult);

            return result;
        }

        private void PrepareSecret(StringBuilder postData)
        {
            postData.Append($"&secret={integrationSystemSetting.Secret}");
        }

        private void PreparePhoneNumber(string phoneNumber, StringBuilder postData, bool forGettingClient = false)
        {
            var propName = forGettingClient ? "client_phone" : "phone";
            postData.Append($"&{propName}={HttpUtility.UrlEncode(ProcessedPhoneNumber(phoneNumber))}");
        }

        private void PrepareProductData(IOrderDetails orderDetails, StringBuilder postData)
        {
            var order = orderDetails.GetOrder();
            var products = new List<string>();
            var productCount = new List<int>();
            var productPrice = new List<double>();

            if (order.ProductCount != null && order.ProductCount.Any())
            {
                foreach (var productId in order.ProductCount.Keys)
                {
                    var product = orderDetails.GetProduct(productId);
                    products.Add(product.VendorCode);
                    productCount.Add(order.ProductCount[productId]);
                    productPrice.Add(product.Price);
                }
            }

            if (order.ProductBonusCount != null && order.ProductBonusCount.Any())
            {
                foreach (var productId in order.ProductBonusCount.Keys)
                {
                    var product = orderDetails.GetProduct(productId);
                    products.Add(product.VendorCode);
                    productCount.Add(order.ProductBonusCount[productId]);
                    productPrice.Add(0);
                }
            }

            for (var i = 0; i < products.Count; ++i)
            {
                postData.Append($"&product[{i}]={products[i]}");
                postData.Append($"&product_kol[{i}]={productCount[i]}");
                postData.Append($"&product_price[{i}]={productPrice[i]}");
            }
        }

        private void PrepareOrderData(IOrderDetails orderDetails, StringBuilder postData)
        {
            var order = orderDetails.GetOrder();
            var buyType = order.BuyType == BuyType.Cash ? BuyType.Cash : BuyType.Card;
           
            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            var amountPayCashback = order.AmountPayCashBack.ToString(nfi);
            
            postData.Append($"&street={HttpUtility.UrlEncode(order.Street)}");
            postData.Append($"&home={HttpUtility.UrlEncode(order.HomeNumber)}");
            postData.Append($"&pod={HttpUtility.UrlEncode(order.EntranceNumber)}");
            postData.Append($"&et={HttpUtility.UrlEncode(order.Level)}");
            postData.Append($"&apart={HttpUtility.UrlEncode(order.ApartamentNumber)}");
            postData.Append($"&descr={HttpUtility.UrlEncode(order.Comment)}");
            postData.Append($"&name={HttpUtility.UrlEncode(order.Name)}");
            postData.Append($"&score={HttpUtility.UrlEncode(amountPayCashback)}");
            postData.Append($"&pay={HttpUtility.UrlEncode(((int)buyType).ToString())}");
            postData.Append($"&person={HttpUtility.UrlEncode(order.NumberAppliances.ToString())}");

            if (order.DateDelivery.HasValue)
            {
                postData.Append($"&datetime={HttpUtility.UrlEncode(order.DateDelivery.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            }

            if (order.DiscountPercent != 0)
            {
                postData.Append($"&sale={HttpUtility.UrlEncode(order.DiscountPercent.ToString())}");
            }
            else if (order.DiscountRuble != 0)
            {
                postData.Append($"&sale_amount={HttpUtility.UrlEncode(order.DiscountRuble.ToString())}");
            }

            PreparePhoneNumber(order.PhoneNumber, postData);
        }

        private void PrepareHookUrlData(StringBuilder postData)
        {
            var hookStatus = "";
            var hoolStatusList = new List<int>
            {
                frontPadOptions.StatusNew,
                frontPadOptions.StatusProcessed,
                frontPadOptions.StatusDelivery,
                frontPadOptions.StatusDone,
                frontPadOptions.StatusCancel
            };

            for (var i = 0; i < hoolStatusList.Count; ++i)
            {
                hookStatus += $"&hook_status[{i}]={hoolStatusList[i]}";
            }

            var domainUrl = HttpContext.Current.Request.Url.GetBaseUrl();
            postData.Append(hookStatus);
            postData.Append($"&hook_url={HttpUtility.UrlEncode($"{domainUrl}/api/frontpad/changestatus")}");
        }

        private void PrepareSalePointData(IOrderDetails orderDetails, StringBuilder postData)
        {
            var order = orderDetails.GetOrder();
            var pointSale = order.DeliveryType == DeliveryType.Delivery ?
                frontPadOptions.PointSaleDelivery : 
                frontPadOptions.PointSaleTakeyourself;

            if(pointSale != 0)
                postData.Append($"&point={pointSale}");
        }

        private string ProcessedPhoneNumber(string phoneNumber)
        {

            var phoneChars = phoneNumber.ToArray();
            Action<int> addCountryCode = startIndex =>
            {
                var tmpChars = new List<Char>(frontPadOptions.PhoneCodeCountry.ToList());
                for (var i = startIndex; i < phoneChars.Length; ++i)
                    tmpChars.Add(phoneChars[i]);

                phoneChars = tmpChars.ToArray();
            };

            if (frontPadOptions.UsePhoneMask)
            {
                var startIdex = phoneChars.ToList().IndexOf('(');
                addCountryCode(startIdex);
            }
            else 
            {
                phoneChars = phoneChars.Where(p => Char.IsDigit(p)).ToArray();
                addCountryCode(1);
            }

            var processedPhoneNumber = new string(phoneChars);

            return processedPhoneNumber;
        }
    }
}