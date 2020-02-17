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

        public void SendMessage(FMCMessage message)
        {
            try
            {
                var notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = message.Title,
                    Body = message.Body
                };

                var fcmMessage = new Message()
                {
                    Notification = notification,
                    Data = message.Data,
                    Topic = topicName,
                    Android = new AndroidConfig { Notification = new AndroidNotification { ImageUrl = message.ImageUrl } },
                    Apns = new ApnsConfig { FcmOptions = new ApnsFcmOptions { ImageUrl = message.ImageUrl } }
                };

                FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }
    }
}