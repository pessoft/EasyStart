using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Logic.Services.Order;
using EasyStart.Models;
using EasyStart.Repositories;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class OrderService
    {
        private readonly IOrderLogic orderLogic;

        public OrderService(IOrderLogic orderLogic)
        {
            this.orderLogic = orderLogic;
        }

        public OrderModel Get(int id)
        {
            return orderLogic.Get(id);
        }

        public OrderModel GetByExternalId(long id)
        {
            return orderLogic.GetByExternalId(id);
        }

        public void MarkOrderSendToIntegrationSystem(int orderId, INewOrderResult orderResult)
        {
            orderLogic.MarkOrderSendToIntegrationSystem(orderId, orderResult);
        }

        public List<OrderModel> GetOrdersForClient(int clinetId)
        {
            return orderLogic.GetOrdersForClient(clinetId);
        }

        public void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status, DateTime updateDate)
        {
            orderLogic.ChangeIntegrationStatus(orderId, status, updateDate);
        }

        public void ChangeInnerStatus(int orderId, OrderStatus status, DateTime updateDate)
        {
            orderLogic.ChangeInnerStatus(orderId, status, updateDate);
        }

        public void UpdateCommentCauseCancel(int orderId, string commentCauseCancel)
        {
            orderLogic.UpdateCommentCauseCancel(orderId, commentCauseCancel);
        }

        public TimeSpan GetAverageOrderProcessingTime(int branchId, DeliveryType deliveryType, TimeSpan defaultOrderProessingTime, string zoneId)
        {
            return orderLogic.GetAverageOrderProcessingTime(branchId, deliveryType, defaultOrderProessingTime, zoneId);
        }
    }
}