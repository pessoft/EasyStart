using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Logic.Services.Order;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Models.ProductOption;
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
            var orderLogic = new OrderLogic(orderRepository);
            orderService = new OrderService(orderLogic);

            var productRepository = new ProductRepository(context);
            var additionalFillingRepository = new AdditionalFillingRepository(context);
            var additionOptionItemRepository = new AdditionOptionItemRepository(context);
            var productAdditionalFillingRepository = new DefaultRepository<ProductAdditionalFillingModal>(context);
            var productAdditionOptionItemRepository = new DefaultRepository<ProductAdditionalOptionModal>(context);
            productService = new ProductService(
                productRepository,
                additionalFillingRepository,
                additionOptionItemRepository,
                productAdditionalFillingRepository,
                productAdditionOptionItemRepository);

            var deliverySettingRepository = new DeliverySettingRepository(context);
            var areaDeliveryRepository = new AreaDeliveryRepository(context);
            deliverySettingService = new DeliverySettingService(deliverySettingRepository, areaDeliveryRepository);

            var fCMDeviceRepository = new FCMDeviceRepository(context);
            pushNotificationService = new PushNotificationService(fCMDeviceRepository, fcmAuthKeyPath);
        }

        public void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status)
        {
            var order = orderService.Get(orderId);
            var deliverSetting = deliverySettingService.GetByBranchId(order.BranchId);
            var dateUpdate = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

            orderService.ChangeIntegrationStatus(orderId, status, dateUpdate);
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
                else if (IsProductsValidForIntegrationSystem(products, order))
                {
                    var deliverySetting = deliverySettingService.GetByBranchId(order.BranchId);
                    var additionalFillings = productService.GetAdditionalFillingsByBranchId(order.BranchId);
                    var additionOptionItems = productService.GetAdditionOptionItemByBranchId(order.BranchId);
                    var orderDetails = new OrderDetails(
                        order,
                        products,
                        additionalFillings,
                        additionOptionItems,
                        deliverySetting.AreaDeliveries);
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

        private bool IsProductsValidForIntegrationSystem(
            List<ProductModel> products,
            OrderModel order)
        {
            if (products == null || !products.Any())
                return false;

            var additionalFillings = productService.GetAdditionalFillingsByBranchId(order.BranchId).ToDictionary(p => p.Id);
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
                } else if (string.IsNullOrEmpty(product.VendorCode))
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
            var deliverySetting = deliverySettingService.GetByBranchId(branchId);
            var defaultOrderProessingTime = TimeSpan.Parse(deliverySetting.MinTimeProcessingOrder);

            return orderService.GetAverageOrderProcessingTime(branchId, deliveryType, defaultOrderProessingTime, deliverySetting.ZoneId);
        }
    }
}