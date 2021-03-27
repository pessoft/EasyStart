using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
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

        public OrderModel GetByExternalId(long id)
        {
            var result = repository.Get(p => p.IntegrationOrderId == id && p.IntegrationOrderId != 0)
                .FirstOrDefault();

            return result;
        }

        public void MarkOrderSendToIntegrationSystem(int orderId, INewOrderResult orderResult)
        {
            if (!orderResult.Success)
                return;

            var order = Get(orderId);
            order.IntegrationOrderId = orderResult.ExternalOrderId;
            order.IntegrationOrderNumber = orderResult.OrderNumber;
            order.IsSendToIntegrationSystem = true;
            order.IntegrationOrderStatus = IntegrationOrderStatus.New;

            repository.Update(order);
        }

        public List<OrderModel> GetOrdersForClient(int clinetId)
        {
            var orders = repository.Get(p => p.ClientId == clinetId).ToList();

            return orders;
        }

        public void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status)
        {
            var order = Get(orderId);
            order.IntegrationOrderStatus = status;

            repository.Update(order);
        }

        public void ChangeInnerStatus(int orderId, OrderStatus status, DateTime updateDate)
        {
            var order = Get(orderId);
            order.OrderStatus = status;
            order.UpdateDate = updateDate;

            repository.Update(order);
        }

        public void UpdateCommentCauseCancel(int orderId, string commentCauseCancel)
        {
            if (commentCauseCancel == null)
                return;

            var order = Get(orderId);
            order.CommentCauseCancel = commentCauseCancel;

            repository.Update(order);
        }
    }
}