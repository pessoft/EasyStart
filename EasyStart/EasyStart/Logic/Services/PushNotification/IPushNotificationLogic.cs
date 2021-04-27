using EasyStart.Logic.IntegrationSystem;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.PushNotification
{
    public interface IPushNotificationLogic
    {
        void ChangeOrderStatus(IntegrationOrderStatus orderStatus, OrderModel order);
        void ChangeOrderStatus(
            IntegrationOrderStatus orderStatus,
            OrderModel order,
            EasyStart.Models.FCMNotification.PushNotification message);
    }
}
