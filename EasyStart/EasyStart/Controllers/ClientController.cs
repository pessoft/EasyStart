using EasyStart.JsResult;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.GeneralSettings;
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
            var pushNotificationLogic = new PushNotificationLogic(repositoryFactory);
            var branchLogic = new BranchLogic(repositoryFactory);
            var clientLogic = new ClientLogic(repositoryFactory);
            var generalSettingLogic = new GeneralSettingsLogic(repositoryFactory);
            var categoryProductLogic = new CategoryProductLogic(
                repositoryFactory,
                imageLogic,
                displayItemSettingLogic);

            orderService = new OrderService(
                orderLogic,
                integrationSystemLogic,
                productLogic,
                deliverySettingLogic,
                pushNotificationLogic);
            productService = new ProductService(productLogic, branchLogic, categoryProductLogic);
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