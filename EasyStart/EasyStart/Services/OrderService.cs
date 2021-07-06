using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Repository;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Services
{
    public class OrderService
    {
        private readonly IOrderLogic orderLogic;
        private readonly IIntegrationSystemLogic integrationSystemLogic;
        private readonly IProductLogic productLogic;
        private readonly IDeliverySettingLogic deliverySettingLogic;
        private readonly IPushNotificationLogic pushNotificationLogic;

        public OrderService(
            IOrderLogic orderLogic,
            IIntegrationSystemLogic integrationSystemLogic,
            IProductLogic productLogic,
            IDeliverySettingLogic deliverySettingLogic,
            IPushNotificationLogic pushNotificationLogic)
        {
            this.orderLogic = orderLogic;
            this.integrationSystemLogic = integrationSystemLogic;
            this.productLogic = productLogic;
            this.deliverySettingLogic = deliverySettingLogic;
            this.pushNotificationLogic = pushNotificationLogic;
        }

        public OrderModel Get(int id)
        {
            return orderLogic.Get(id);
        }

        public OrderModel GetByExternalId(long id)
        {
            return orderLogic.GetByExternalId(id);
        }

        public IEnumerable<OrderModel> GetByBranchIds(IEnumerable<int> branchIds)
        {
            return orderLogic.GetByBranchIds(branchIds);
        }

        public TodayDataOrdersModel GetDataOrdersByDate(IEnumerable<int> branchIds, DateTime date)
        {
            return orderLogic.GetDataOrdersByDate(branchIds, date);
        }

        public IEnumerable<OrderModel> GetHistory(HistoryOrderFilter filter)
        {
            return orderLogic.GetHistory(filter);
        }

        public void MarkOrderSendToIntegrationSystem(int orderId, INewOrderResult orderResult)
        {
            orderLogic.MarkOrderSendToIntegrationSystem(orderId, orderResult);
        }

        public List<OrderModel> GetOrdersForClient(int clinetId)
        {
            return orderLogic.GetOrdersForClient(clinetId);
        }

        public void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status)
        {
            var order = orderLogic.Get(orderId);
            var deliverSetting = deliverySettingLogic.GetByBranchId(order.BranchId);
            var dateUpdate = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

            if (order.DeliveryType == DeliveryType.TakeYourSelf
                && order.IntegrationOrderStatus == IntegrationOrderStatus.Prepared
                && status == IntegrationOrderStatus.Done)
            {
                dateUpdate = order.UpdateDate;
            }

            orderLogic.ChangeIntegrationStatus(orderId, status, dateUpdate);
        }

        public void ChangeInnerStatus(int orderId, OrderStatus status, DateTime updateDate)
        {
            orderLogic.ChangeInnerStatus(orderId, status, updateDate);
        }

        public void UpdateCommentCauseCancel(int orderId, string commentCauseCancel)
        {
            orderLogic.UpdateCommentCauseCancel(orderId, commentCauseCancel);
        }

        public TimeSpan GetAverageOrderProcessingTime(int branchId, DeliveryType deliveryType)
        {
            var deliverySetting = deliverySettingLogic.GetByBranchId(branchId);
            var defaultOrderProessingTime = TimeSpan.Parse(deliverySetting.MinTimeProcessingOrder);

            return orderLogic.GetAverageOrderProcessingTime(branchId, deliveryType, defaultOrderProessingTime, deliverySetting.ZoneId);
        }

        public void ChangeOrderStatus(UpdaterOrderStatus payload)
        {
            if (payload.Status == OrderStatus.Processing || payload.OrderId < 1)
                return;

            var order = orderLogic.Get(payload.OrderId);
            if (order.OrderStatus != OrderStatus.Processing)
                return;

            var deliverSetting = deliverySettingLogic.GetByBranchId(order.BranchId);
            payload.DateUpdate = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

            orderLogic.ChangeInnerStatus(order.Id, payload.Status, payload.DateUpdate);

            if (payload.Status == OrderStatus.Processed)
            {
                new PromotionLogic().ProcessingVirtualMoney(payload.OrderId, order.BranchId);
            }
            else if (payload.Status == OrderStatus.Cancellation)
            {
                orderLogic.UpdateCommentCauseCancel(order.Id, payload.CommentCauseCancel);

                new PromotionLogic().Refund(payload.OrderId);

                if (order.IntegrationOrderStatus == IntegrationOrderStatus.Unknown)
                    Task.Run(() => pushNotificationLogic.ChangeOrderStatus(IntegrationOrderStatus.Canceled, order));

            }
        }

        public INewOrderResult SendOrderToIntegrationSystem(int orderId)
        {
            var order = orderLogic.Get(orderId);
            INewOrderResult newOrderResult;

            if (order.IsSendToIntegrationSystem)
                newOrderResult = NewOrderResult.GetFakeSuccessResult(order.IntegrationOrderNumber, order.IntegrationOrderId);
            else
            {
                var products = productLogic.Get(order);

                if (!IsValidOrderDiscount(order))
                    newOrderResult = new NewOrderResult("Discount persent and discount ruble no supported integraion system");
                else if (IsProductsValidForIntegrationSystem(products, order))
                {
                    var deliverySetting = deliverySettingLogic.GetByBranchId(order.BranchId);
                    var additionalFillings = productLogic.GetAdditionalFillingsByBranchId(order.BranchId);
                    var additionOptionItems = productLogic.GetAdditionOptionItemByBranchId(order.BranchId);
                    var orderDetails = new OrderDetails(
                        order,
                        products,
                        additionalFillings,
                        additionOptionItems,
                        deliverySetting.AreaDeliveries);
                    newOrderResult = integrationSystemLogic.SendOrder(
                        orderDetails,
                        new IntegrationSystemFactory());

                    orderLogic.MarkOrderSendToIntegrationSystem(orderId, newOrderResult);
                }
                else
                    newOrderResult = new NewOrderResult("Not all products valid for sending integration system");
            }

            return newOrderResult;
        }

        private bool IsProductsValidForIntegrationSystem(
            List<ProductModel> products,
            OrderModel order)
        {
            if (products == null || !products.Any())
                return false;

            var additionalFillings = productLogic.GetAdditionalFillingsByBranchId(order.BranchId).ToDictionary(p => p.Id);
            foreach (var product in products)
            {
                if (product.ProductAdditionalFillingIds != null && product.ProductAdditionalFillingIds.Any()
                    || product.ProductAdditionalOptionIds != null && product.ProductAdditionalOptionIds.Any())
                {
                    var productWithOptions = order.ProductWithOptionsCount.FirstOrDefault(p => p.ProductId == product.Id);
                    var isFillingValid = true;
                    if (productWithOptions.AdditionalFillings != null && productWithOptions.AdditionalFillings.Any())
                    {
                        foreach (var additionalFillingId in productWithOptions.AdditionalFillings)
                        {
                            var additionalFilling = additionalFillings[additionalFillingId];
                            if (string.IsNullOrEmpty(additionalFilling.VendorCode))
                            {
                                isFillingValid = false;
                                break;
                            }
                        }
                    }

                    var isValidAdditionalOptions = true;
                    if (productWithOptions.AdditionalOptions != null && productWithOptions.AdditionalOptions.Any())
                    {
                        var optionIds = productWithOptions.AdditionalOptions.Values.OrderBy(p => p).ToArray();
                        var optionIdsStr = string.Join("-", optionIds);

                        if (product.AllowCombinationsVendorCode != null && product.AllowCombinationsVendorCode.Any())
                        {
                            string combinationVendorCode = null;
                            foreach (var kv in product.AllowCombinationsVendorCode)
                            {
                                var combinatationStre = string.Join("-", kv.Value.OrderBy(p => p));

                                if (combinatationStre == optionIdsStr)
                                {
                                    combinationVendorCode = kv.Key;
                                    break;
                                }
                            }

                            isValidAdditionalOptions = !string.IsNullOrEmpty(combinationVendorCode);
                            if (!isValidAdditionalOptions || !isFillingValid)
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(product.VendorCode) || !isFillingValid)
                            return false;
                    }
                }
                else if (string.IsNullOrEmpty(product.VendorCode))
                    return false;
            }

            return true;
        }

        private bool IsValidOrderDiscount(OrderModel order)
        {
            return !(order.DiscountPercent > 0 && order.DiscountRuble > 0);
        }
    }
}