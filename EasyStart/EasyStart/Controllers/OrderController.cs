using EasyStart.JsResult;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.Controllers
{
    public class OrderController : Controller
    {
        private readonly BranchService branchService;
        private readonly ClientService clientService;
        private readonly OrderService orderService;
        private readonly ProductService productService;

        public OrderController()
        {
            var context = new AdminPanelContext();

            var orderRepository = new OrderRepository(context);
            var orderLogic = new OrderLogic(orderRepository);

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            var integrationSystemLogic = new IntegrationSystemLogic(inegrationSystemRepository);

            var productRepository = new ProductRepository(context);
            var additionalFillingRepository = new AdditionalFillingRepository(context);
            var additionOptionItemRepository = new AdditionOptionItemRepository(context);
            var productAdditionalFillingRepository = new DefaultRepository<ProductAdditionalFillingModal>(context);
            var productAdditionOptionItemRepository = new DefaultRepository<ProductAdditionalOptionModal>(context);
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

            var branchRepository = new BranchRepository(context);
            var branchLogic = new BranchLogic(branchRepository);

            var clientRepository = new ClientRepository(context);
            var clientLogic = new ClientLogic(clientRepository);

            orderService = new OrderService(
                orderLogic,
                integrationSystemLogic,
                productLogic,
                deliverySettingLogic,
                pushNotificationLogic);
            productService = new ProductService(productLogic);
            clientService = new ClientService(clientLogic);
            branchService = new BranchService(branchLogic);
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetOrdersForClient(int clientId)
        {
            var result = new JsonResultModel();

            try
            {
                var orders = orderService.GetOrdersForClient(clientId);

                result.Success = true;
                result.Data = orders;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При получении списка заказов что-то пошло не так.";
            }

            return this.JsResult(result);
        }
    }
}