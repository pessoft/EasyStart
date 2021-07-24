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
    public class OrderController : Controller
    {
        private readonly BranchService branchService;
        private readonly ClientService clientService;
        private readonly OrderService orderService;
        private readonly ProductService productService;

        public OrderController()
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