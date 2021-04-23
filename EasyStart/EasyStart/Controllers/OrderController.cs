using EasyStart.JsResult;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.Product;
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

            var branchRepository = new BranchRepository(context);
            var branchLogic = new BranchLogic(branchRepository);
            branchService = new BranchService(branchLogic);

            var clientRepository = new ClientRepository(context);
            clientService = new ClientService(clientRepository);

            var orderRepository = new OrderRepository(context);
            var orderLogic = new OrderLogic(orderRepository);
            orderService = new OrderService(orderLogic);

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
            productService = new ProductService(productLogic);
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