using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.OrderProcessor;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Order;
using EasyStart.Models;
using EasyStart.Models.Integration;
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
        private readonly IOrderProcesser orderProcessor;

        public FrontPadController()
        {
            var context = new AdminPanelContext();

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            var integrationSystemLogic = new IntegrationSystemLogic(inegrationSystemRepository);
            integrationSystemService = new IntegrationSystemService(integrationSystemLogic);

            var orderRepository = new OrderRepository(context);
            var orderLogic = new OrderLogic(orderRepository);
            orderService = new OrderService(orderLogic);

            var fcmDeviveRepository = new FCMDeviceRepository(context);
            var fcmAuthKeyPath = HostingEnvironment.MapPath("/Resource/FCMAuthKey.json");
            pushNotificationService = new PushNotificationService(fcmDeviveRepository, fcmAuthKeyPath);

            orderProcessor = new OrderProcessor();
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

            orderProcessor.ChangeIntegrationStatus(order.Id, orderStatus);

            if (integrationSetting.UseAutomaticDispatch
                && (orderStatus == IntegrationOrderStatus.Done || orderStatus == IntegrationOrderStatus.Canceled))
            {
                var updatePayload = new UpdaterOrderStatus
                {
                    OrderId = order.Id,
                    Status = orderStatus == IntegrationOrderStatus.Done ? OrderStatus.Processed : OrderStatus.Cancellation
                };

                orderProcessor.ChangeOrderStatus(updatePayload);
                new OrderHub().ChangeOrderStatus(order.Id, updatePayload.Status, order.BranchId);
            }

            pushNotificationService.ChangeOrderStatus(orderStatus, order);

            return status;
        }
    }
}