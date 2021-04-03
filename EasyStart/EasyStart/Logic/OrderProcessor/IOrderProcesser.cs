using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.OrderProcessor
{
    public interface IOrderProcesser
    {
        void ChangeOrderStatus(UpdaterOrderStatus payload);
        void ChangeIntegrationStatus(int orderId, IntegrationOrderStatus status);
        INewOrderResult SendOrderToIntegrationSystem(int orderId);
        TimeSpan GetAverageOrderProcessingTime(int branchId, DeliveryType deliveryType);
    }
}