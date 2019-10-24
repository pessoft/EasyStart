using EasyStart.HtmlRenderer.Models;
using EasyStart.Logic.Notification.EmailNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Notification
{
    public class OptionsNotificationNewOrderModel
    {
        public OrderModel Order { get; set; }
        public OrderIfnoModel OrderInfo { get; set; }
        public string DomainUr {get;set;}
        public List<string> ToEmail { get; set; }
        public IEmail Email { get; set; }
        public string EmailBodyHTMLTemplate { get; set; }
    }
}