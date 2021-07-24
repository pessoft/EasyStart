using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.PushNotification
{
    public class PushNotificationInfo
    {
        public PushNotificationInfo(
            int limitPushMessageToday,
            int countMessagesSentToday)
        {
            LimitPushMessageToday = limitPushMessageToday;
            CountMessagesSentToday = countMessagesSentToday;
        }

        public int LimitPushMessageToday { get; private set; }
        public int CountMessagesSentToday { get; private set; }
    }
}