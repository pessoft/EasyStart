using EasyStart.Logic.FCM;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace EasyStart.Logic.Services.PushNotification
{
    public class PushNotificationLogic: IPushNotificationLogic
    {
        private static readonly int LIMIT_PUSH_MESSAGE_TODAY = 5;
        private readonly int PAGE_PUSH_MESSAGE_SIZE = 10;

        private readonly IRepository<FCMDeviceModel, int> fcmDeviceRepository;
        private readonly IRepository<PushMessageModel, int> pushMessageRepository;

        private readonly int branchId;

        private static readonly string fcmAuthKeyPath = HostingEnvironment.MapPath("/Resource/FCMAuthKey.json");

        public PushNotificationLogic(
            IRepositoryFactory repositoryFactory,
            IBranchLogic branchLogic)
        {
            fcmDeviceRepository = repositoryFactory.GetRepository<FCMDeviceModel, int>();
            pushMessageRepository = repositoryFactory.GetRepository<PushMessageModel, int>();
            
            branchId = branchLogic.Get().Id;
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

        public PushNotificationInfo PushNotification(
            Models.FCMNotification.PushNotification pushNotification,
            string uriDomain)
        {
            var message = new FCMMessage(pushNotification);

            if (string.IsNullOrEmpty(message.Title))
                throw new PushNotifictionException("Отсутствует заголовок сообщения");
            else if (string.IsNullOrEmpty(message.Body))
                throw new PushNotifictionException("Отсутствует тело сообщения");

            var countMessagesSentToday = GetCountMessagesSentToday();
            if (countMessagesSentToday >= LIMIT_PUSH_MESSAGE_TODAY)
                throw new PushNotifictionException("Превышен дневной лимит push уведомлений");

            var pushMessage = new PushMessageModel(message, branchId, DateTime.Now.Date);//время не нужно
            pushMessageRepository.Create(pushMessage);

            if (!string.IsNullOrEmpty(message.ImageUrl))
                message.ImageUrl = uriDomain + message.ImageUrl.Substring(2);

            SendMulticastMessage(message);

            return GetPushNotificationInfo();
        }

        public PushNotificationInfo GetPushNotificationInfo()
        {
            var countMessagesSentToday = GetCountMessagesSentToday();
            return new PushNotificationInfo(
                LIMIT_PUSH_MESSAGE_TODAY,
                ++countMessagesSentToday
                );
        }

        public PagingPushMessageHistory GetPagingPushMessageHistory(int pageNumber)
        {
            var messagesCount = pushMessageRepository.Get(p => p.BranchId == branchId).Count();
            var messagesMaxPage = messagesCount == 0 ? 1 : Convert.ToInt32(Math.Ceiling((double)messagesCount / PAGE_PUSH_MESSAGE_SIZE));
            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            var history = pushMessageRepository
                .Get(p => p.BranchId == branchId)
                .OrderByDescending(p => p.Date)
                .Skip(PAGE_PUSH_MESSAGE_SIZE * (pageNumber - 1))
                .Take(PAGE_PUSH_MESSAGE_SIZE)
                .ToList();

            return new PagingPushMessageHistory
            {
                HistoryMessages = history,
                PageNumber = pageNumber,
                PageSize = PAGE_PUSH_MESSAGE_SIZE,
                IsLast = messagesMaxPage == pageNumber
            };
        }

        private void SendMulticastMessage(FCMMessage message)
        {
            Task.Run(() =>
            {
                var tokens = GetDiveceTokens();
                if (tokens == null || !tokens.Any())
                    return;

                var fcm = new FCMNotification(fcmAuthKeyPath, tokens);
                fcm.SendMulticastMessage(message);
            });
        }

        private int GetCountMessagesSentToday()
        {
            var currentDate = DateTime.Now.Date;
            return pushMessageRepository.Get(p => p.BranchId == branchId
                && p.Date.Date == currentDate)
                .Count();
        }

        private string GetClientDiveceToken(int clinetId)
        {
            var device = fcmDeviceRepository.Get(p => p.ClientId == clinetId).FirstOrDefault();

            return device?.Token;
        }

        private List<string> GetDiveceTokens()
        {
            return fcmDeviceRepository.Get(p => p.BranchId == branchId)
                .Select(p => p.Token)
                .ToList(); ;
        }
    }
}