using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using System;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.Order
{
    public interface IOrderLogic
    {
        OrderModel Get(int id);
        OrderModel GetByExternalId(long id);
        IEnumerable<OrderModel> GetByBranchIds(IEnumerable<int> branchIds);
        TodayDataOrdersModel GetDataOrdersByDate(IEnumerable<int> branchIds, DateTime date);
        IEnumerable<OrderModel> GetHistory(HistoryOrderFilter filter);
        void MarkOrderSendToIntegrationSystem(int orderId, INewOrderResult orderResult);
        List<OrderModel> GetOrdersForClient(int clinetId);
        void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status, DateTime updateDate);
        void ChangeInnerStatus(int orderId, OrderStatus status, DateTime updateDate);
        void UpdateCommentCauseCancel(int orderId, string commentCauseCancel);
        TimeSpan GetAverageOrderProcessingTime(int branchId, DeliveryType deliveryType, TimeSpan defaultOrderProessingTime, string zoneId);
    }
}
