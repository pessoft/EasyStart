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
            .Replace("{customerNumberPhone}", orderInfo.CustomerNumberPhone)
            .Replace("{orderDateDelivery}", orderInfo.OrderDateDelivery)
            .Replace("{numberAppliances}", orderInfo.NumberAppliances);
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
            var defaultProducts = OrderDefaultProductsRender();
            var defaultBonusProducts = OrderDefaultBonusProductsRender();
            var constructorProducts = OrderConstructorProductsRender();
            var productsWithOptions = OrderProductsWithOptionsRender();
            var bonusProductsWithOptions = OrderBonusProductsWithOptionsRender();

            body.Replace("{productsInfo}", $"{defaultProducts} {productsWithOptions} {constructorProducts} {defaultBonusProducts} {bonusProductsWithOptions}");
        }


        private string OrderDefaultProductsRender()
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.Products)
            {
                prodcutsInfo.Append($"<tr style='height: 25px;'><td style='min-width: 110px;'>{product.ProductName}</td><td style='text-align: right; min-width: 100px;'>{product.ProductCount} х {product.ProductPrice} руб.</td></tr>");

            }

            return prodcutsInfo.ToString();
        }

        private string OrderDefaultBonusProductsRender()
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.BonusProducts)
            {
                prodcutsInfo.Append($"<tr style='height: 25px; color: #FF5722;'><td style='min-width: 110px;'>{product.ProductName}</td><td style='text-align: right; min-width: 100px;'>{product.ProductCount} х 0 руб.</td></tr>");
            }

            return prodcutsInfo.ToString();
        }

        private string OrderConstructorProductsRender()
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.ConstructorProducts)
            {
                prodcutsInfo.Append($"<tr style='height: 25px;'><td style='min-width: 110px;'>{product.ProductName}</td><td style='text-align: right; min-width: 100px;'>{product.ProductCount} х {product.ProductPrice} руб.</td></tr>");

                foreach (var ingredient in product.Ingredients)
                {
                    prodcutsInfo.Append($"<tr style='height: 25px; font-size: 0.85em'><td style='min-width: 110px; padding-left: 12px; color: #6d758a;'>{ingredient.Name}</td><td style='text-align: right; color: #6d758a; min-width: 100px;'>{ingredient.Count} х {ingredient.Price} руб.</td></tr>");
                }
            }

            return prodcutsInfo.ToString();
        }

        private string OrderProductsWithOptionsRender()
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.ProductsWithOptions)
            {
                prodcutsInfo.Append($"<tr style='height: 25px;'><td style='min-width: 110px;'>{product.ProductName}</td><td style='text-align: right; min-width: 100px;'>{product.ProductCount} х {product.ProductPrice} руб.</td></tr>");

                foreach (var additionalOptionItem in product.AdditionalOptions.SelectMany(p => p.Items))
                {
                    prodcutsInfo.Append($"<tr style='height: 25px; font-size: 0.85em'><td style='min-width: 110px; padding-left: 12px; color: #6d758a;'>{additionalOptionItem.Name}</td><td style='text-align: right; color: #6d758a; min-width: 100px;'>{additionalOptionItem.Price} руб.</td></tr>");
                }

                foreach (var additionalFilling in product.AdditionalFillings)
                {
                    prodcutsInfo.Append($"<tr style='height: 25px; font-size: 0.85em'><td style='min-width: 110px; padding-left: 12px; color: #6d758a;'>{additionalFilling.Name}</td><td style='text-align: right; color: #6d758a; min-width: 100px;'>{additionalFilling.Price} руб.</td></tr>");
                }
            }

            return prodcutsInfo.ToString();
        }

        private string OrderBonusProductsWithOptionsRender()
        {
            var prodcutsInfo = new StringBuilder();

            foreach (var product in orderInfo.BonusProductsWithOptions)
            {
                prodcutsInfo.Append($"<tr style='height: 25px; color: #FF5722;'><td style='min-width: 110px;'>{product.ProductName}</td><td style='text-align: right; min-width: 100px;'>{product.ProductCount} х {product.ProductPrice} руб.</td></tr>");

                foreach (var additionalOptionItem in product.AdditionalOptions.SelectMany(p => p.Items))
                {
                    prodcutsInfo.Append($"<tr style='height: 25px; font-size: 0.85em'><td style='min-width: 110px; padding-left: 12px; color: #6d758a;'>{additionalOptionItem.Name}</td><td style='text-align: right; color: #6d758a; min-width: 100px;'>{additionalOptionItem.Price} руб.</td></tr>");
                }

                foreach (var additionalFilling in product.AdditionalFillings)
                {
                    prodcutsInfo.Append($"<tr style='height: 25px; font-size: 0.85em'><td style='min-width: 110px; padding-left: 12px; color: #6d758a;'>{additionalFilling.Name}</td><td style='text-align: right; color: #6d758a; min-width: 100px;'>{additionalFilling.Price} руб.</td></tr>");
                }
            }

            return prodcutsInfo.ToString();
        }

        private void OrderPriceRender(StringBuilder body)
        {
            var markPay = orderInfo.BuyType == Logic.BuyType.Online ? "Оплачено " : "";
            var orderCheckoutPrice = $"<tr style='height: 25px;'><td style='min-width: 110px;'>Сумма заказа</td><td style='text-align: right; min-width: 100px;'>{orderInfo.AmountPrice}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Оплачено бонусами</td><td style='text-align: right; min-width: 100px;'>{orderInfo.AmountPayCashBack}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Стоимость доставки</td><td style='text-align: right; min-width: 100px;'>{orderInfo.DeliveryPrice}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Скидка</td><td style='text-align: right; min-width: 100px;'>{orderInfo.Discount}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Способ оплаты</td><td style='text-align: right; min-width: 100px;'>{orderInfo.BuyTypeName}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>Сдача c</td><td style='text-align: right; min-width: 100px;'>{orderInfo.CashBack}</td></tr>" +
              $"<tr style='height: 25px;'><td style='min-width: 110px;'>К оплате</td><td style='text-align: right; min-width: 100px;'>{markPay}{orderInfo.AmountPayDiscountDelivery}</td></tr>";

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