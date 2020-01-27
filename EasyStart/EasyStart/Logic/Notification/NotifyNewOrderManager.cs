using EasyStart.HtmlRenderer;
using EasyStart.Hubs;
using EasyStart.Logic.Notification.EmailNotification;
using EasyStart.Models;
using EasyStart.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Notification
{
    public class NotifyNewOrderManager
    {
        private readonly OptionsNotificationNewOrderModel options;

        public NotifyNewOrderManager(OptionsNotificationNewOrderModel options)
        {
            this.options = options;
        }

        public void AllNotify()
        {
            AdminPanelInnerNotify();
            EmailNotify();
        }

        public void AdminPanelInnerNotify()
        {
            new NewOrderHub().AddedNewOrder(options.Order);
        }

        public void EmailNotify() {
            if (options.ToEmail == null || !options.ToEmail.Any())
                return;

            var emailNotification = new OrderEmailNotification(options.Order.Id, options.Email, options.ToEmail);
            var orderBodyHtmlRenderer = new EmailOrderBodyHtmlRenderer(options.OrderInfo, options.DomainUr, options.EmailBodyHTMLTemplate);

            emailNotification.EmailNotify(orderBodyHtmlRenderer);
        }
    }
}