using EasyStart.JsResult;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Models;
using EasyStart.Models.Integration;
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
            branchService = new BranchService(branchRepository);

            var clientRepository = new ClientRepository(context);
            clientService = new ClientService(clientRepository);

            var orderRepository = new OrderRepository(context);
            orderService = new OrderService(orderRepository);

            var productRepository = new ProductRepository(context);
            var additionalFillingRepository = new AdditionalFillingRepository(context);
            var additionOptionItemRepository = new AdditionOptionItemRepository(context);
            productService = new ProductService(
                productRepository,
                additionalFillingRepository,
                additionOptionItemRepository);
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