using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public FrontPad(IntegrationSystemModel integrationSystemSetting): base(integrationSystemSetting)
        { }

        public override INewOrderResult SendOrder(IOrderDetails orderDetails)
        {
            var postData = new StringBuilder();

            PrepareSecret(postData);
            PrepareProductData(orderDetails, postData);
            PrepareOrderData(orderDetails, postData);

            var result = SendOrder(postData.ToString());

            return result;
        }

        private INewOrderResult SendOrder(string postData)
        {
            string responseResult = base.SendOrder(newOrderUrl, postData).Result;
            var frontPadResult = JsonConvert.DeserializeObject<FrontPadNewOrderResult>(responseResult);
            var result = new NewOrderResult(frontPadResult);

            return result;
        }

        private void PrepareSecret(StringBuilder postData)
        {
            postData.Append($"&secret={integrationSystemSetting.Secret}");
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
            var phoneNumber = new String(order.PhoneNumber.Where(p => Char.IsDigit(p)).ToArray());

            postData.Append($"&street={HttpUtility.UrlEncode(order.Street)}");
            postData.Append($"&home={HttpUtility.UrlEncode(order.HomeNumber)}");
            postData.Append($"&apart={HttpUtility.UrlEncode(order.ApartamentNumber)}");
            postData.Append($"&phone={HttpUtility.UrlEncode(phoneNumber)}");
            postData.Append($"&descr={HttpUtility.UrlEncode(order.Comment)}");
            postData.Append($"&name={HttpUtility.UrlEncode(order.Name)}");
        }
    }
}