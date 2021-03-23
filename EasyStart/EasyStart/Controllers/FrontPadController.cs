using EasyStart.Logic.IntegrationSystem;
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

        public FrontPadController()
        {
            var context = new AdminPanelContext();

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            integrationSystemService = new IntegrationSystemService(inegrationSystemRepository);

            var orderRepository = new OrderRepository(context);
            orderService = new OrderService(orderRepository);

            var fcmDeviveRepository = new FCMDeviceRepository(context);
            var fcmAuthKeyPath = HostingEnvironment.MapPath("/Resource/FCMAuthKey.json");
            pushNotificationService = new PushNotificationService(fcmDeviveRepository, fcmAuthKeyPath);
        }

        [HttpPost]
        public FrontPadOrderStatus ChangeStatus([FromBody] FrontPadOrderStatus status)
        {
            if (status.OrderId == 0)
                return status;

            var order = this.orderService.GetByExternalId(status.OrderId);
            var integrationSetting = this.integrationSystemService.Get(order.BranchId);

            if (integrationSetting.Type != Logic.IntegrationSystemType.FrontPad)
                return status;

            var integerationSystem = new IntegrationSystemFactory().GetIntegrationSystem(integrationSetting);
            var orderStatus = integerationSystem.GetIntegrationOrderStatus(status.Status);
            
            this.orderService.ChangeIntegrationStatus(order.Id, orderStatus);
            order = this.orderService.Get(order.Id);

            this.pushNotificationService.ChangeOrderStatus(orderStatus, order);

            return status;
        }
    }
}