using EasyStart.Logic.FCM;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class PushNotificationService
    {
        private readonly IRepository<FCMDeviceModel> repository;
        private readonly string fcmAuthKeyPath;

        public PushNotificationService(IRepository<FCMDeviceModel> repository, string fcmAuthKeyPath)
        {
            this.repository = repository;
            this.fcmAuthKeyPath = fcmAuthKeyPath;
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
                    bodyMsg = "Заказ отменен";
                    break;
            }

            var message = new PushNotification
            {
                Title = $"Инофрмация о заказке #{order.Id}",
                Body = bodyMsg
            };

            ChangeOrderStatus(orderStatus, order, message);
        }

        /// <summary>
        /// Use for status Preparig or Deliverid
        /// </summary>
        /// <param name="orderStatus"></param>
        public void ChangeOrderStatus(IntegrationOrderStatus orderStatus, OrderModel order, PushNotification message)
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