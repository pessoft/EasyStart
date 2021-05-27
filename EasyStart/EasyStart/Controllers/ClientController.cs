using EasyStart.JsResult;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.GeneralSettings;
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
    public class ClientController : Controller
    {
        private readonly BranchService branchService;
        private readonly ClientService clientService;
        private readonly OrderService orderService;
        private readonly ProductService productService;

        public ClientController()
        {
            var context = new AdminPanelContext();
            var imageLogic = new ContainImageLogic();

            var orderRepository = new OrderRepository(context);
            var orderLogic = new OrderLogic(orderRepository);

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            var integrationSystemLogic = new IntegrationSystemLogic(inegrationSystemRepository);

            var productRepository = new ProductRepository(context);
            var additionalFillingRepository = new AdditionalFillingRepository(context);
            var additionOptionItemRepository = new AdditionOptionItemRepository(context);
            var productAdditionalFillingRepository = new ProductAdditionalFillingRepository(context);
            var productAdditionOptionItemRepository = new ProductAdditionOptionItemRepository(context);
            var additionalOptionRepository = new AdditionalOptionRepository(context);
            var productLogic = new ProductLogic(
                productRepository,
                additionalFillingRepository,
                additionalOptionRepository,
                additionOptionItemRepository,
                productAdditionalFillingRepository,
                productAdditionOptionItemRepository,
                imageLogic);

            var deliverySettingRepository = new DeliverySettingRepository(context);
            var areaDeliverySettingRepository = new AreaDeliveryRepository(context);
            var deliverySettingLogic = new DeliverySettingLogic(deliverySettingRepository, areaDeliverySettingRepository);

            var fcmDeviveRepository = new FCMDeviceRepository(context);
            var pushNotificationLogic = new PushNotificationLogic(fcmDeviveRepository);

            var branchRepository = new BranchRepository(context);
            var branchLogic = new BranchLogic(branchRepository);

            var clientRepository = new ClientRepository(context);
            var clientLogic = new ClientLogic(clientRepository);

            var generalSettingRepository = new GeneralSettingRepository(context);
            var generalSettingLogic = new GeneralSettingsLogic(generalSettingRepository);

            orderService = new OrderService(
                orderLogic,
                integrationSystemLogic,
                productLogic,
                deliverySettingLogic,
                pushNotificationLogic);
            productService = new ProductService(productLogic, branchLogic);
            clientService = new ClientService(clientLogic);
            branchService = new BranchService(branchLogic, generalSettingLogic);
        }

        [HttpGet]
        [Authorize]
        public JsonResult Get()
        {
            var result = new JsonResultModel();

            try
            {
                var branch = branchService.Get(User.Identity.Name);
                var clients = clientService.GetAll(branch.Id);

                result.Success = true;
                result.Data = clients;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При получении списка клиентов что-то пошло не так.";
            }

            return this.JsResult(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Block(int clientId)
        {
            var result = new JsonResultModel();

            try
            {
                clientService.Block(clientId);

                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При блокировки клиента что-то пошло не так.";
            }

            return this.JsResult(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UnBlock(int clientId)
        {
            var result = new JsonResultModel();

            try
            {
                clientService.UnBlock(clientId);

                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При разблокировки клиента что-то пошло не так.";
            }

            return this.JsResult(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SetVirtualMoney(int clientId, double virtualMoney)
        {
            var result = new JsonResultModel();

            try
            {
                clientService.SetVirtualMoney(clientId, virtualMoney);

                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При обновлении бонусных средств что-то пошло не так.";
            }

            return this.JsResult(result);
        }
    }
}