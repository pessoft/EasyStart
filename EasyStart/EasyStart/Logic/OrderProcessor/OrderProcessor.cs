using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Repositories;
using EasyStart.Services;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace EasyStart.Logic.OrderProcessor
{
    public class OrderProcessor : IOrderProcesser
    {
        private readonly IntegrationSystemService integrationSystemService;
        private readonly BranchService branchService;
        private readonly OrderService orderService;
        private readonly ProductService productService;
        private readonly DeliverySettingService deliverySettingService;
        private readonly PushNotificationService pushNotificationService;
        private readonly static string fcmAuthKeyPath;

        static OrderProcessor()
        {
            fcmAuthKeyPath = HostingEnvironment.MapPath("/Resource/FCMAuthKey.json");
        }

        public OrderProcessor()
        {
            var context = new AdminPanelContext();

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            integrationSystemService = new IntegrationSystemService(inegrationSystemRepository);

            var branchRepository = new BranchRepository(context);
            branchService = new BranchService(branchRepository);

            var orderRepository = new OrderRepository(context);
            orderService = new OrderService(orderRepository);

            var productRepository = new ProductRepository(context);
            productService = new ProductService(productRepository);

            var deliverySettingRepository = new DeliverySettingRepository(context);
            var areaDeliveryRepository = new AreaDeliveryRepository(context);
            deliverySettingService = new DeliverySettingService(deliverySettingRepository, areaDeliveryRepository);

            var fCMDeviceRepository = new FCMDeviceRepository(context);
            pushNotificationService = new PushNotificationService(fCMDeviceRepository, fcmAuthKeyPath);
        }

        public void ChangeOrderStatus(UpdaterOrderStatus payload)
        {
            if (payload.Status == OrderStatus.Processing || payload.OrderId < 1)
                return;

            var order = orderService.Get(payload.OrderId);
            if (order.OrderStatus != OrderStatus.Processing)
                return;

            var deliverSetting = deliverySettingService.GetByBranchId(order.BranchId);
            payload.DateUpdate = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

            orderService.ChangeInnerStatus(order.Id, payload.Status, payload.DateUpdate);

            if (payload.Status == OrderStatus.Processed)
            {
                new PromotionLogic().ProcessingVirtualMoney(payload.OrderId, order.BranchId);
            }
            else if (payload.Status == OrderStatus.Cancellation)
            {
                orderService.UpdateCommentCauseCancel(order.Id, payload.CommentCauseCancel);

                new PromotionLogic().Refund(payload.OrderId);

                if (order.IntegrationOrderStatus == IntegrationOrderStatus.Unknown)
                    Task.Run(() => pushNotificationService.ChangeOrderStatus(IntegrationOrderStatus.Canceled, order));

            }
        }

        public INewOrderResult SendOrderToIntegrationSystem(int orderId)
        {
            var order = orderService.Get(orderId);
            INewOrderResult newOrderResult;

            if (order.IsSendToIntegrationSystem)
                newOrderResult = NewOrderResult.GetFakeSuccessResult(order.IntegrationOrderNumber, order.IntegrationOrderId);
            else
            {
                var products = productService.Get(order);

                if(!IsValidOrderDiscount(order))
                    newOrderResult = new NewOrderResult("Discount persent and discount ruble no supported integraion system");
                else if (IsProductsValidForIntegrationSystem(products))
                {
                    var orderDetails = new OrderDetails(order, products);
                    newOrderResult = integrationSystemService.SendOrder(
                        orderDetails,
                        new IntegrationSystemFactory());

                    orderService.MarkOrderSendToIntegrationSystem(orderId, newOrderResult);
                }
                else
                    newOrderResult = new NewOrderResult("Not all products valid for sending integration system");
            }

            return newOrderResult;
        }

        private bool IsProductsValidForIntegrationSystem(List<ProductModel> products)
        {
            if (products == null || !products.Any())
                return false;

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.VendorCode)
                    || product.ProductAdditionalFillingIds != null && product.ProductAdditionalFillingIds.Any()
                    || product.ProductAdditionalOptionIds != null && product.ProductAdditionalOptionIds.Any())
                    return false;
            }

            return true;
        }

        private bool IsValidOrderDiscount(OrderModel order)
        {
            return !(order.DiscountPercent > 0 && order.DiscountRuble > 0);
        }

        public TimeSpan GetAverageOrderProcessingTime(int branchId, DeliveryType deliveryType)
        {
            var minTimeProcessingOrder = deliverySettingService.GetByBranchId(branchId);
            var defaultOrderProessingTime = TimeSpan.Parse(minTimeProcessingOrder.MinTimeProcessingOrder);

            return orderService.GetAverageOrderProcessingTime(branchId, deliveryType, defaultOrderProessingTime);
        }
    }
}