using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using EasyStart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyStart.Controllers
{
    /// <summary>
    /// Web hook controller
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FrontPadController : ApiController
    {
        private readonly OrderService orderService;
        private readonly PushNotificationService pushNotificationService;
        private readonly IntegrationSystemService integrationSystemService;

        public FrontPadController()
        {
            var context = new AdminPanelContext();

            var orderRepository = new OrderRepository(context);
            var orderLogic = new OrderLogic(orderRepository);

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            var integrationSystemLogic = new IntegrationSystemLogic(inegrationSystemRepository);

            var productRepository = new ProductRepository(context);
            var additionalFillingRepository = new AdditionalFillingRepository(context);
            var additionOptionItemRepository = new AdditionOptionItemRepository(context);
            var productAdditionalFillingRepository = new BaseRepository<ProductAdditionalFillingModal, int>(context);
            var productAdditionOptionItemRepository = new BaseRepository<ProductAdditionalOptionModal, int>(context);
            var productLogic = new ProductLogic(
                productRepository,
                additionalFillingRepository,
                additionOptionItemRepository,
                productAdditionalFillingRepository,
                productAdditionOptionItemRepository);

            var deliverySettingRepository = new DeliverySettingRepository(context);
            var areaDeliverySettingRepository = new AreaDeliveryRepository(context);
            var deliverySettingLogic = new DeliverySettingLogic(deliverySettingRepository, areaDeliverySettingRepository);

            var fcmDeviveRepository = new FCMDeviceRepository(context);
            var pushNotificationLogic = new PushNotificationLogic(fcmDeviveRepository);

            orderService = new OrderService(
                orderLogic,
                integrationSystemLogic,
                productLogic,
                deliverySettingLogic,
                pushNotificationLogic);
            integrationSystemService = new IntegrationSystemService(integrationSystemLogic);
            pushNotificationService = new PushNotificationService(pushNotificationLogic);
            integrationSystemService = new IntegrationSystemService(integrationSystemLogic);
        }

        [HttpPost]
        public FrontPadOrderStatus ChangeStatus([FromBody] FrontPadOrderStatus status)
        {
            var order = orderService.GetByExternalId(status.OrderId);

            if (status.OrderId == 0 || order == null)
                return status;

            var integrationSetting = integrationSystemService.Get(order.BranchId);

            if (integrationSetting.Type != Logic.IntegrationSystemType.FrontPad)
                return status;

            var integerationSystem = new IntegrationSystemFactory().GetIntegrationSystem(integrationSetting);
            var orderStatus = integerationSystem.GetIntegrationOrderStatus(status.Status);

            orderService.ChangeIntegrationStatus(order.Id, orderStatus);

            if (integrationSetting.UseAutomaticDispatch
                && (orderStatus == IntegrationOrderStatus.Done || orderStatus == IntegrationOrderStatus.Canceled))
            {
                var updatePayload = new UpdaterOrderStatus
                {
                    OrderId = order.Id,
                    Status = orderStatus == IntegrationOrderStatus.Done ? OrderStatus.Processed : OrderStatus.Cancellation
                };

                orderService.ChangeOrderStatus(updatePayload);
                new OrderHub().ChangeOrderStatus(order.Id, updatePayload.Status, order.BranchId);
            }

            pushNotificationService.ChangeOrderStatus(orderStatus, order);

            return status;
        }
    }
}