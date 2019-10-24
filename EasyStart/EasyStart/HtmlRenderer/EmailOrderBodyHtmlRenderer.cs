﻿using EasyStart.HtmlRenderer.Models;
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
            var deliveryType = $"<tr style='height: 25px;'><td style='min-width: 110px;'>Способ доставки</td><td style='text-align: right;'>{orderInfo.DeliveryTypeStr}</td></tr>";
            var baseAddress = orderInfo.DeliveryType == Logic.DeliveryType.TakeYourSelf ? "" :
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Город</td><td style='text-align: right;'>{orderInfo.City}</td></tr>" +
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Улица</td><td style='text-align: right;'>{orderInfo.Street}</td></tr>" +
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Дом(офис)</td><td style='text-align: right;'>{orderInfo.HomeNumber}</td></tr>" +
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Номер квартиры</td><td style='text-align: right;'>{orderInfo.ApartmentNumber}</td></tr>" +
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Этаж</td><td style='text-align: right;'>{orderInfo.Level}</td></tr>" +
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Номер подъезда</td><td style='text-align: right;'>{orderInfo.EntranceNumber}</td></tr>" + 
                $"<tr style='height: 25px;'><td style='min-width: 110px;'>Код домафона</td><td style='text-align: right;'>{orderInfo.IntercomCode}</td></tr>";

            body.Replace("{orderAddress}", deliveryType + baseAddress);
        }

        private void OrderProductsRender(StringBuilder body)
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.Products)
            {
                prodcutsInfo.Append($"<tr style='height: 25px;'><td style='min-width: 110px;'>{product.ProductName}</td><td style='text-align: right;'>{product.ProductCount} х {product.ProductPrice} руб.</td></tr>");
            }

            body.Replace("{productsInfo}", prodcutsInfo.ToString());
        }

        private void OrderPriceRender(StringBuilder body)
        {
            var orderCheckoutPrice = $"<tr style='height: 25px;'><td style='min-width: 110px;'>Сумма заказа</td><td style='text-align: right;'>{orderInfo.AmountPrice}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Стоимость доставки</td><td style='text-align: right;'>{orderInfo.DeliveryPrice}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Скидка</td><td style='text-align: right;'>{orderInfo.Discount}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Способ оплаты</td><td style='text-align: right;'>{orderInfo.ButType}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Сдача c</td><td style='text-align: right;'>{orderInfo.CashBack}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>К оплате</td><td style='text-align: right;'>{orderInfo.AmountPayDiscountDelivery}</td></tr>";

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