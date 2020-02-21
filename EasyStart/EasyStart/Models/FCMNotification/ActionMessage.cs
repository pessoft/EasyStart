using EasyStart.Logic.FCM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.FCMNotification
{
    public class ActionMessage
    {
        public NotificationActionType Type { get; set; }
        public int TargetId { get; set; }
    }
}