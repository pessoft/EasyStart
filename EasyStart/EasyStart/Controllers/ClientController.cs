﻿using EasyStart.Logic.IntegrationSystem;
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
    public class ClientController : Controller
    {
        private readonly BranchService branchService;
        private readonly ClientService clientService;
        private readonly OrderService orderService;
        private readonly ProductService productService;

        public ClientController()
        {
            var context = new AdminPanelContext();

            var branchRepository = new BranchRepository(context);
            branchService = new BranchService(branchRepository);

            var clientRepository = new ClientRepository(context);
            clientService = new ClientService(clientRepository);

            var orderRepository = new OrderRepository(context);
            orderService = new OrderService(orderRepository);

            var productRepository = new ProductRepository(context);
            productService = new ProductService(productRepository);
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}