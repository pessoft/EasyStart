using EasyStart.HtmlRenderer.Models;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Utils;

namespace EasyStart.Logic
{
    public static class OrderHelper
    {
        public static OrderIfnoModel GetOrderInfo(
            this OrderModel order,
            SettingModel setting,
            List<ProductModel> products)
        {
            var orderInfo = new OrderIfnoModel();

            SetMetaData(orderInfo, order);
            SetCustomer(orderInfo, order);
            SetAddress(orderInfo, order, setting);
            SetProducts(orderInfo, order, products);
            SetComment(orderInfo, order);
            SetPrice(orderInfo, order);

            return orderInfo;
        }

        private static void SetMetaData(OrderIfnoModel orderInfo, OrderModel order)
        {
            orderInfo.OrderNumber = order.Id.ToString();
            orderInfo.OrderDate = order.Date.ToString("HH:mm dd.MM.yyyy");
        }

        private static void SetCustomer(OrderIfnoModel orderInfo, OrderModel order)
        {
            orderInfo.CustomerName = order.Name;
            orderInfo.CustomerNumberPhone = order.PhoneNumber;
        }

        private static void SetAddress(OrderIfnoModel orderInfo, OrderModel order, SettingModel setting)
        {
            orderInfo.DeliveryType = order.DeliveryType;
            orderInfo.DeliveryTypeStr = order.DeliveryType.GetDescription();
            orderInfo.City = setting.City;
            orderInfo.Street = order.Street;
            orderInfo.HomeNumber = order.HomeNumber;
            orderInfo.ApartmentNumber = order.ApartamentNumber;
            orderInfo.Level = order.Level;
            orderInfo.IntercomCode = order.IntercomCode;
            orderInfo.EntranceNumber = order.EntranceNumber;
        }

        private static void SetProducts(OrderIfnoModel orderInfo, OrderModel order, List<ProductModel> products)
        {
            orderInfo.Products = new List<ProductInfoModel>();

            foreach (var product in products)
            {
                orderInfo.Products.Add(new ProductInfoModel
                {
                    ProductCount = order.ProductCount[product.Id],
                    ProductName = product.Name,
                    ProductPrice = product.Price
                });
            }
        }

        private static void SetPrice(OrderIfnoModel orderInfo, OrderModel order)
        {
            orderInfo.AmountPrice = $"{order.AmountPay} руб.";
            orderInfo.DeliveryPrice = $"{order.DeliveryPrice} руб.";
            orderInfo.Discount = order.DiscountPercent == 0 ? $"{order.DiscountPercent}%" : $"{order.DiscountPercent}% ({Math.Round(order.AmountPay * order.DiscountPercent / 100, 2)} руб.)";
            orderInfo.ButType = order.BuyType.GetDescription();
            orderInfo.CashBack = order.CashBack == 0 ? $"{order.CashBack} руб." : $"{order.CashBack} руб. ({Math.Round(order.CashBack - order.AmountPayDiscountDelivery, 2)} руб.)";
            orderInfo.AmountPayDiscountDelivery = $"{order.AmountPayDiscountDelivery} руб."; ;
        }

        private static void SetComment(OrderIfnoModel orderInfo, OrderModel order)
        {
            orderInfo.Comment = string.IsNullOrEmpty(order.Comment) || string.IsNullOrEmpty(order.Comment.Trim()) ? "Отсутсвтует" : order.Comment;
        }
    }
}