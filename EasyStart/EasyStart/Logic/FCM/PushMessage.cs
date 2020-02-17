using EasyStart.Models.FCMNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.FCM
{
    public class PushMessage
    {
        private FCMNotification fcm;
        public PushMessage(string fcmAuthKeyPath, string fcmTopicName)
        {
            fcm = new FCMNotification(fcmAuthKeyPath, fcmTopicName);
        }

        public void SendMessage(FMCMessage message)
        {

            fcm.SendMessage(message);
        }
    }
}