using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.FCMNotification
{
    public class PushNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public ActionMessage Action { get; set;}
    }
}