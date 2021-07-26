using EasyStart.Logic.IntegrationSystem;
using EasyStart.Models;

namespace EasyStart.Logic.Services.PushNotification
{
    public interface IPushNotificationLogic
    {
        void ChangeOrderStatus(IntegrationOrderStatus orderStatus, OrderModel order);
        void ChangeOrderStatus(
            IntegrationOrderStatus orderStatus,
            OrderModel order,
            EasyStart.Models.FCMNotification.PushNotification message);

        PushNotificationInfo PushNotification(
            EasyStart.Models.FCMNotification.PushNotification pushNotification,
            string uriDomain);

        PushNotificationInfo GetPushNotificationInfo();
    }
}
