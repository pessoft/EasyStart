using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class OrderService
    {
        private readonly IRepository<OrderModel> repository;
        
        public OrderService(IRepository<OrderModel> repository)
        {
            this.repository = repository;
        }

        public OrderModel Get(int id)
        {
            var result = repository.Get(id);

            return result;
        }

        public void MarkOrderSendToIntegrationSystem(int orderId, INewOrderResult orderResult)
        {
            if (!orderResult.Success)
                return;

            var order = Get(orderId);
            order.IntegrationOrderNumber = orderResult.OrderNumber;
            order.IsSendToIntegrationSystem = true;

            repository.Update(order);
        }
    }
}