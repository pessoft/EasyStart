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

            if (externalOrderStatusId == frontPadOptions.StatusNew)
                status = IntegrationOrderStatus.New;
            else if (externalOrderStatusId == frontPadOptions.StatusProcessed)
                status = IntegrationOrderStatus.Preparing;
            else if (externalOrderStatusId == frontPadOptions.StatusDelivery)
                status = IntegrationOrderStatus.Deliverid;
            else if (externalOrderStatusId == frontPadOptions.StatusCancel)
                status = IntegrationOrderStatus.Canceled;
            else if (externalOrderStatusId == frontPadOptions.StatusDone)
                status = IntegrationOrderStatus.Done;
            else if (externalOrderStatusId == frontPadOptions.StatusPrepared)
                status = IntegrationOrderStatus.Prepared;

            return status;
        }

        public override INewOrderResult SendOrder(IOrderDetails orderDetails)
        {
            var postData = new StringBuilder();

            PrepareSecret(postData);
            PrepareProductAndFakeProductAreaDelievryData(orderDetails, postData);
            PrepareOrderData(orderDetails, postData);
            PrepareSalePointData(orderDetails, postData);
            PrepareAffiliateData(postData);
            PrepareHookUrlData(postData);

            var result = SendOrder(postData.ToString());

            return result;
        }

        public override double GetClinetVirtualMoney(string phoneNumber)
        {
            var postData = new StringBuilder();

            PrepareSecret(postData);
            PreparePhoneNumber(phoneNumber, postData, true);

            string responseResult = base.Post(getClientUrl, postData.ToString());
            var frontpadClientVirtualMoney = JsonConvert.DeserializeAnonymousType(responseResult, new { Score = 0.0 });

            return frontpadClientVirtualMoney.Score;
        }

        private INewOrderResult SendOrder(string postData)
        {
            string responseResult = base.Post(newOrderUrl, postData);
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

        private void PrepareProductAndFakeProductAreaDelievryData(IOrderDetails orderDetails, StringBuilder postData)
        {
            var order = orderDetails.GetOrder();
            var products = new List<string>();
            var productCount = new List<int>();
            var productPrice = new List<double>();
            var productMod = new Dictionary<int, int>();

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

            if (order.ProductWithOptionsCount != null && order.ProductWithOptionsCount.Any())
            {
                foreach (var productWithOptionsCount in order.ProductWithOptionsCount)
                {
                    var product = orderDetails.GetProduct(productWithOptionsCount.ProductId);
                    var price = product.Price;
                    string vendorCode = null;

                    if (productWithOptionsCount.AdditionalOptions != null
                        && productWithOptionsCount.AdditionalOptions.Any())
                    {
                        var optionItemIds = productWithOptionsCount.AdditionalOptions.Values.ToList();
                        foreach (var optionItemId in optionItemIds)
                        {
                            var optionItem = orderDetails.GetAdditionOptionItem(optionItemId);
                            price += optionItem.Price;
                        }

                        if (product.AllowCombinationsVendorCode == null
                            || !product.AllowCombinationsVendorCode.Any())
                            throw new Exception("No AllowCombinationsVendorCode");

                        var optionItemIdsStr = string.Join("-", optionItemIds.OrderBy(p => p));
                        foreach (var kv in product.AllowCombinationsVendorCode)
                        {
                            var optionItemIdsStrFind = string.Join("-", kv.Value.OrderBy(p => p));

                            if (optionItemIdsStr == optionItemIdsStrFind)
                            {
                                vendorCode = kv.Key;
                            }
                        }

                        if (string.IsNullOrEmpty(vendorCode))
                            throw new Exception("Not find vendorCode for additional option");
                    }

                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        if(string.IsNullOrEmpty(product.VendorCode))
                            throw new Exception($"Not find vendorCode for product id = {product.Id}");

                        vendorCode = product.VendorCode;
                    }

                    products.Add(vendorCode);
                    productCount.Add(productWithOptionsCount.Count);
                    productPrice.Add(price);

                    var productIndex = products.Count - 1;

                    if (productWithOptionsCount.AdditionalFillings != null
                        && productWithOptionsCount.AdditionalFillings.Any())
                    {
                        foreach (var additionalFillingId in productWithOptionsCount.AdditionalFillings)
                        {
                            var additionalFilling = orderDetails.GetAdditionalFilling(additionalFillingId);

                            products.Add(additionalFilling.VendorCode);
                            productCount.Add(productWithOptionsCount.Count);
                            productPrice.Add(additionalFilling.Price);

                            var modIndex = products.Count - 1; ;

                            productMod.Add(modIndex, productIndex);
                        }
                    }
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

            var areaVendorCode = orderDetails.GetAreaDeliveryCode();
            if (!string.IsNullOrEmpty(areaVendorCode))
            {
                products.Add(areaVendorCode);
                productCount.Add(1);
                productPrice.Add(order.DeliveryPrice);
            }

            for (var i = 0; i < products.Count; ++i)
            {
                postData.Append($"&product[{i}]={products[i]}");
                postData.Append($"&product_kol[{i}]={productCount[i]}");
                postData.Append($"&product_price[{i}]={productPrice[i]}");
            }

            if (productMod.Any())
            {
                foreach(var kv in productMod)
                    postData.Append($"&product_mod[{kv.Key}]={kv.Value}");
            }
        }

        private void PrepareOrderData(IOrderDetails orderDetails, StringBuilder postData)
        {
            var order = orderDetails.GetOrder();
            int buyType;

            if (order.BuyType == BuyType.Cash)
                buyType = (int)BuyType.Cash;
            else if (order.BuyType == BuyType.Online && frontPadOptions.OnlineBuyType != 0)
                buyType = frontPadOptions.OnlineBuyType;
            else
                buyType = (int)BuyType.Card;

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
            postData.Append($"&pay={HttpUtility.UrlEncode((buyType).ToString())}");
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
            var hookStatusList = new List<int>
            {
                frontPadOptions.StatusNew,
                frontPadOptions.StatusProcessed,
                frontPadOptions.StatusDelivery,
                frontPadOptions.StatusDone,
                frontPadOptions.StatusCancel,
                frontPadOptions.StatusPrepared
            };

            for (var i = 0; i < hookStatusList.Count; ++i)
            {
                hookStatus += $"&hook_status[{i}]={hookStatusList[i]}";
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

        private void PrepareAffiliateData(StringBuilder postData)
        {
            if (frontPadOptions.Affiliate != 0)
                postData.Append($"&affiliate={frontPadOptions.Affiliate}");
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