using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.PushNotification
{
    public class PushNotifictionException : Exception
    {
        public PushNotifictionException(string message) : base(message)
        { }
    }
}