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
        private static int LIMIT_TOKENS = 100;

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

        public void SendMessageForTopic(FCMMessage message)
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
                if (this.tokens == null || !this.tokens.Any())
                    return;

                var notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = message.Title,
                    Body = message.Body,
                    ImageUrl = message.ImageUrl
                };

                AndroidConfig androidConfig = new AndroidConfig { Notification = new AndroidNotification { Sound = "default" } };
                ApnsConfig apnsConfig = new ApnsConfig { Aps = new Aps { Sound = "default", Alert = new ApsAlert { LaunchImage = message.ImageUrl } } };
                var sleepTimeMs = 100;

                var countRepeatSendMsg =Convert.ToInt32(Math.Ceiling((double)tokens.Count / LIMIT_TOKENS));

                for (var i = 1; i <= countRepeatSendMsg; ++i)
                {
                    var packageTokens = tokens.Skip((i - 1) * LIMIT_TOKENS)
                        .Take(LIMIT_TOKENS)
                        .ToList();
                    var fcmMessage = new MulticastMessage()
                    {
                        Notification = notification,
                        Data = message.Data,
                        Tokens = packageTokens,
                        Android = androidConfig,
                        Apns = apnsConfig
                    };

                    FirebaseMessaging.DefaultInstance.SendMulticastAsync(fcmMessage);

                    Thread.Sleep(sleepTimeMs);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }
    }
}