﻿using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
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
        private readonly IDefaultEntityRepository<OrderModel> repository;

        public OrderService(IDefaultEntityRepository<OrderModel> repository)
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

        public void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status, DateTime updateDate)
        {
            var order = Get(orderId);
            order.IntegrationOrderStatus = status;
            order.UpdateDate = updateDate;

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

        public TimeSpan GetAverageOrderProcessingTime(int branchId, DeliveryType deliveryType, TimeSpan defaultOrderProessingTime, string zoneId)
        {
            var timeProcessingOrders = repository.Get(p =>
                p.BranchId == branchId
                && p.IsSendToIntegrationSystem
                && p.IntegrationOrderStatus == IntegrationOrderStatus.Done
                && p.DeliveryType == deliveryType
                && p.Date.Date == DateTime.Now.Date
                && p.DateDelivery == null)
                .Select(p => p.UpdateDate.GetDateTimeNow(zoneId) - p.Date.GetDateTimeNow(zoneId));
            var orderCount = timeProcessingOrders.Count();

            if (orderCount == 0)
                return defaultOrderProessingTime;

            var aggregateTimeProcessingOrder = timeProcessingOrders.Aggregate((t1, t2) => t1 + t2);
            var timeProcessingOrder = TimeSpan.FromTicks(aggregateTimeProcessingOrder.Ticks / orderCount);

            return timeProcessingOrder;
        }
    }
}