using EasyStart.Logic.FCM;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class PushNotificationService
    {
        private readonly IPushNotificationLogic pushNotificationLogic;

        public PushNotificationService(IPushNotificationLogic pushNotificationLogic)
        {
            this.pushNotificationLogic = pushNotificationLogic;
        }

        /// <summary>
        /// Use for status Preparig or Deliverid
        /// </summary>
        /// <param name="orderStatus"></param>
        public void ChangeOrderStatus(IntegrationOrderStatus orderStatus, OrderModel order)
        {
            pushNotificationLogic.ChangeOrderStatus(orderStatus, order);
        }

        /// <summary>
        /// Use for status Preparig or Deliverid
        /// </summary>
        /// <param name="orderStatus"></param>
        public void ChangeOrderStatus(IntegrationOrderStatus orderStatus, OrderModel order, PushNotification message)
        {
            pushNotificationLogic.ChangeOrderStatus(orderStatus, order, message);
        }
    }
}