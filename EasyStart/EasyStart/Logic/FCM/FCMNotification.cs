using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.IO;
using EasyStart.Models.FCMNotification;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;

namespace EasyStart.Logic.FCM
{
    public class FCMNotification
    {
        //Имя пакета приложениея
        private string topicName;
        private List<string> tokens;

        public FCMNotification(string serviceAccountKeyPath, string topicNamePath)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                    });

                topicName = File.ReadAllText(topicNamePath);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

        }

        public FCMNotification(string serviceAccountKeyPath, List<string> tokens)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                    });

                this.tokens = tokens;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

        }

        public void SendMessage(FCMMessage message)
        {
            try
            {
                var notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = message.Title,
                    Body = message.Body
                };

                AndroidConfig androidConfig = string.IsNullOrEmpty(message.ImageUrl) ? null : new AndroidConfig { Notification = new AndroidNotification { ImageUrl = message.ImageUrl } };
                ApnsConfig apnsConfig = string.IsNullOrEmpty(message.ImageUrl) ? null : new ApnsConfig { FcmOptions = new ApnsFcmOptions { ImageUrl = message.ImageUrl } };

                var fcmMessage = new Message()
                {
                    Notification = notification,
                    Data = message.Data,
                    Topic = topicName,
                    Android = androidConfig,
                    Apns = apnsConfig
                };

                FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public void SendMulticastMessage(FCMMessage message)
        {
            try
            {
                message.ImageUrl = "https://es-admin-10.ru/Images/Products/7fdb659c-151d-40dc-9abb-99e5442dbdfe.jpg";
                var notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = message.Title,
                    Body = message.Body,
                    ImageUrl = message.ImageUrl
                };

                ApnsConfig apnsConfig = new ApnsConfig { Aps = new Aps { Sound = "default" } };

                var fcmMessage = new MulticastMessage()
                {
                    Notification = notification,
                    Data = message.Data,
                    Tokens = tokens,
                    Apns = apnsConfig
                };

                FirebaseMessaging.DefaultInstance.SendMulticastAsync(fcmMessage);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }
    }
}