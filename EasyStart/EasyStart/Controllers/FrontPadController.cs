using EasyStart.Hubs;
using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Models.ProductOption;
using EasyStart.Repository;
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
            var repositoryFactory = new RepositoryFactory(context);
            var imageLogic = new ContainImageLogic();
            var displayItemSettingLogic = new DisplayItemSettingLogic();

            var orderLogic = new OrderLogic(repositoryFactory);
            var integrationSystemLogic = new IntegrationSystemLogic(repositoryFactory);
            var productLogic = new ProductLogic(
                repositoryFactory,
                imageLogic,
                displayItemSettingLogic);
            var deliverySettingLogic = new DeliverySettingLogic(repositoryFactory);
            var branchLogic = new BranchLogic(repositoryFactory);
            var pushNotificationLogic = new PushNotificationLogic(repositoryFactory, branchLogic);

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