using EasyStart.HtmlRenderer.Models;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EasyStart.HtmlRenderer
{
    public class EmailOrderBodyHtmlRenderer : IHtmlRenderer
    {
        private OrderIfnoModel orderInfo;
        private string domainUrl;
        private string emailTemplate;

        public EmailOrderBodyHtmlRenderer(OrderIfnoModel orderInfo, string domainUrl, string emailTemplate)
        {
            this.orderInfo = orderInfo;
            this.domainUrl = domainUrl;
            this.emailTemplate = emailTemplate;
        }

        public string Render()
        {
            var emailBody = new StringBuilder(this.emailTemplate);

            OrderMetaDataRender(emailBody);
            OrderCustomerRender(emailBody);
            OrderAddressRender(emailBody);
            OrderProductsRender(emailBody);
            OrderPriceRender(emailBody);
            OrderCommentRender(emailBody);
            OrderTotalRender(emailBody);

            return emailBody.ToString();
        }

        private void OrderMetaDataRender(StringBuilder body)
        {
            body.Replace("{orderNumber}", orderInfo.OrderNumber);
            body.Replace("{orderDate}", orderInfo.OrderDate);
        }

        private void OrderCustomerRender(StringBuilder body)
        {
            body
            .Replace("{customerName}", orderInfo.CustomerName)
            .Replace("{customerNumberPhone}", orderInfo.CustomerNumberPhone);
        }

        private void OrderAddressRender(StringBuilder body)
        {
            var deliveryType = $"<tr><td>Способ доставки</td><td class='text-right'>{orderInfo.DeliveryTypeStr}</td></tr>";
            var baseAddress = orderInfo.DeliveryType == Logic.DeliveryType.TakeYourSelf ? "" :
                $"<tr><td>Город</td><td class='text-right'>{orderInfo.City}</td></tr>" +
                $"<tr><td>Улица</td><td class='text-right'>{orderInfo.Street}</td></tr>" +
                $"<tr><td>Дом(офис)</td><td class='text-right'>{orderInfo.HomeNumber}</td></tr>" +
                $"<tr><td>Номер квартиры</td><td class='text-right'>{orderInfo.ApartmentNumber}</td></tr>" +
                $"<tr><td>Этаж</td><td class='text-right'>{orderInfo.Level}</td></tr>" +
                $"<tr><td>Номер подъезда</td><td class='text-right'>{orderInfo.EntranceNumber}</td></tr>" + 
                $"<tr><td>Код домафона</td><td class='text-right'>{orderInfo.IntercomCode}</td></tr>";

            body.Replace("{orderAddress}", deliveryType + baseAddress);
        }

        private void OrderProductsRender(StringBuilder body)
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.Products)
            {
                prodcutsInfo.Append($"<tr><td>{product.ProductName}</td><td class='text-right'>{product.ProductCount} х {product.ProductPrice} руб.</td></tr>");
            }

            body.Replace("{productsInfo}", prodcutsInfo.ToString());
        }

        private void OrderPriceRender(StringBuilder body)
        {
            var orderCheckoutPrice = $"<tr><td>Сумма заказа</td><td class='text-right'>{orderInfo.AmountPrice}</td></tr>" +
              $"<tr><td>Стоимость доставки</td><td class='text-right'>{orderInfo.DeliveryPrice}</td></tr>" +
              $"<tr><td>Скидка</td><td class='text-right'>{orderInfo.Discount}</td></tr>" +
              $"<tr><td>Способ оплаты</td><td class='text-right'>{orderInfo.ButType}</td></tr>" +
              $"<tr><td>Сдача c</td><td class='text-right'>{orderInfo.CashBack}</td></tr>" +
              $"<tr><td>К оплате</td><td class='text-right'>{orderInfo.AmountPayDiscountDelivery}</td></tr>";

            body.Replace("{orderCheckoutPrice}", orderCheckoutPrice);
        }

        private void OrderCommentRender(StringBuilder body)
        {
            body.Replace("{orderComment}", orderInfo.Comment);
        }

        private void OrderTotalRender(StringBuilder body)
        {
            body.Replace("{orderPrice}", orderInfo.AmountPayDiscountDelivery);
        }
    }
}