using EasyStart.Logic.FCM;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

namespace EasyStart.Logic.Services.PushNotification
{
    public class PushNotificationLogic: IPushNotificationLogic
    {
        private readonly IRepository<FCMDeviceModel, int> repository;
        private static readonly string fcmAuthKeyPath = HostingEnvironment.MapPath("/Resource/FCMAuthKey.json");

        public PushNotificationLogic(IRepositoryFactory repositoryFactory)
        {
            this.repository = repositoryFactory.GetRepository<FCMDeviceModel, int>();
        }

        /// <summary>
        /// Use for status Preparig or Deliverid
        /// </summary>
        /// <param name="orderStatus"></param>
        public void ChangeOrderStatus(IntegrationOrderStatus orderStatus, OrderModel order)
        {
            var bodyMsg = "";

            switch (orderStatus)
            {
                case IntegrationOrderStatus.Preparing:
                    bodyMsg = "Ваш заказ готовится";
                    break;
                case IntegrationOrderStatus.Deliverid:
                    bodyMsg = "Заказ передан курьеру";
                    break;
                case IntegrationOrderStatus.Canceled:
                    bodyMsg = "Заказ отменён";
                    break;
            }

            var message = new EasyStart.Models.FCMNotification.PushNotification
            {
                Title = $"Информация о заказке #{order.Id}",
                Body = bodyMsg
            };

            ChangeOrderStatus(orderStatus, order, message);
        }

        /// <summary>
        /// Use for status Preparig or Deliverid
        /// </summary>
        /// <param name="orderStatus"></param>
        public void ChangeOrderStatus(
            IntegrationOrderStatus orderStatus,
            OrderModel order,
            EasyStart.Models.FCMNotification.PushNotification message)
        {
            message.Action = new ActionMessage { Type = Logic.FCM.NotificationActionType.OpenOrder, TargetId = order.Id };

            if (orderStatus == IntegrationOrderStatus.Unknown ||
                orderStatus == IntegrationOrderStatus.New ||
                orderStatus == IntegrationOrderStatus.Done)
                return;

            var clinetDeviceToken = GetClientDiveceToken(order.ClientId);

            if (string.IsNullOrEmpty(clinetDeviceToken))
                return;

            var fcm = new FCMNotification(fcmAuthKeyPath, new List<string> { clinetDeviceToken });
            var fcfMessage = new FCMMessage(message);

            fcm.SendMulticastMessage(fcfMessage);
        }

        private string GetClientDiveceToken(int clinetId)
        {
            var device = this.repository.Get(p => p.ClientId == clinetId).FirstOrDefault();

            return device?.Token;
        }
    }
}