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
           OrderInfoParams orderParams)
        {
            var orderInfo = new OrderIfnoModel();

            SetMetaData(orderInfo, order);
            SetCustomer(orderInfo, order);
            SetAddress(orderInfo, order, orderParams.Setting);
            SetProducts(orderInfo, order, orderParams.Products);
            SetBonusProducts(orderInfo, order, orderParams.BonusProducts);
            SetConstructorProducts(orderInfo, order, orderParams.CategoryConstructor, orderParams.ConstructorIngredients);
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
            orderInfo.OrderDateDelivery = order.DateDelivery == null ? "Как можно быстрее" : order.DateDelivery?.ToString("HH:mm dd.MM.yyyy");
            orderInfo.NumberAppliances = order.NumberAppliances.ToString();
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
                    ProductPrice = product.Price,
                });
            }
        }

        private static void SetBonusProducts(OrderIfnoModel orderInfo, OrderModel order, List<ProductModel> bonusProducts)
        {
            orderInfo.BonusProducts = new List<ProductInfoModel>();

            if (order.ProductBonusCount == null)
                return;

            foreach (var product in bonusProducts)
            {
                orderInfo.BonusProducts.Add(new ProductInfoModel
                {
                    ProductCount = order.ProductBonusCount[product.Id],
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                });
            }
        }

        private static void SetConstructorProducts(
            OrderIfnoModel orderInfo,
            OrderModel order,
            List<CategoryModel> categoriesConstructor,
            List<IngredientModel> constructorIngredients)
        {

            orderInfo.ConstructorProducts = new List<ConstructorProductInfoModel>();

            if (order.ProductConstructorCount == null
               || !order.ProductConstructorCount.Any()
               || categoriesConstructor == null
               || !categoriesConstructor.Any()
               || constructorIngredients == null
               || !constructorIngredients.Any())
            {
                return;
            }

            foreach (var orderProductConstructon in order.ProductConstructorCount)
            {
                var category = categoriesConstructor.First(p => p.Id == orderProductConstructon.CategoryId);

                orderInfo.ConstructorProducts.Add(new ConstructorProductInfoModel
                {
                    ProductCount = orderProductConstructon.Count,
                    ProductName = category.Name,
                    ProductPrice = GetConstructorProductPrice(orderProductConstructon.IngrdientCount, constructorIngredients),
                    Ingredients = GetStrIngredientsCount(orderProductConstructon.IngrdientCount, constructorIngredients),
                });
            }
        }

        private static List<IngredientOrderModel> GetStrIngredientsCount(Dictionary<int, int> ingredientsCount, List<IngredientModel> constructorIngredients)
        {
            var ingredients = new List<IngredientOrderModel>();

            foreach (var ingredint in constructorIngredients)
            {
                var count = 0;

                if (ingredientsCount.TryGetValue(ingredint.Id, out count))
                {
                    ingredients.Add(new IngredientOrderModel
                    {
                        Id = ingredint.Id,
                        Name = ingredint.Name,
                        Count = count,
                        Price = ingredint.Price
                    });
                }
            }

            return ingredients;
        }

        private static double GetConstructorProductPrice(Dictionary<int, int> ingredientsCount, List<IngredientModel> constructorIngredients)
        {
            var price = 0.0;

            foreach (var ingredint in constructorIngredients)
            {
                var count = 0;

                if (ingredientsCount.TryGetValue(ingredint.Id, out count))
                {
                    price += count * ingredint.Price;
                }
            }

            return price;
        }

        private static void SetPrice(OrderIfnoModel orderInfo, OrderModel order)
        {
            orderInfo.AmountPrice = $"{order.AmountPay} руб.";
            orderInfo.AmountPayCashBack = $"{order.AmountPayCashBack} руб.";
            orderInfo.DeliveryPrice = $"{order.DeliveryPrice} руб.";
            orderInfo.Discount = GetDiscountStr(order);
            orderInfo.ButType = order.BuyType.GetDescription();
            orderInfo.CashBack = order.CashBack == 0 ? $"{order.CashBack} руб." : $"{order.CashBack} руб. ({Math.Round(order.CashBack - order.AmountPayDiscountDelivery, 2)} руб.)";
            orderInfo.AmountPayDiscountDelivery = $"{order.AmountPayDiscountDelivery} руб."; ;
        }

        private static string GetDiscountStr(OrderModel order)
        {
            var discountPercent = order.DiscountPercent == 0 ? $"{order.DiscountPercent}%" : $"{order.DiscountPercent}% ({Math.Round(order.AmountPay * order.DiscountPercent / 100, 2)} руб.)"; ;
            var discountRuble = order.DiscountRuble > 0 ? $"{Math.Round(order.DiscountRuble, 2) } руб." : "";
            var discount = "";

            if (!string.IsNullOrEmpty(discountPercent)
                && !string.IsNullOrEmpty(discountRuble))
            {
                discount = $"{discountPercent} и {discountRuble}";
            }
            else if (!string.IsNullOrEmpty(discountPercent))
            {
                discount = discountPercent;
            }
            else if (!string.IsNullOrEmpty(discountRuble))
            {
                discount = discountRuble;
            }

            return discount;
        }

        private static void SetComment(OrderIfnoModel orderInfo, OrderModel order)
        {
            orderInfo.Comment = string.IsNullOrEmpty(order.Comment) || string.IsNullOrEmpty(order.Comment.Trim()) ? "Отсутсвтует" : order.Comment;
        }
    }
}