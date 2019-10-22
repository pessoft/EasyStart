using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EasyStart.Logic.EmailNotification
{
    public class OrderNotification
    {
        private OrderModel order;
        private IEmail email;
        private string toEmail;

        public OrderNotification(OrderModel order, IEmail email, List<string> toEmails)
        {
            this.order = order;
            this.email = email;
            this.toEmail = string.Join(",", toEmails);
        }

        public void EmailNotify()
        {
            var mailMessage = getMailMessage();

            email.Send(mailMessage);
        }

        private MailMessage getMailMessage()
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = $"Заказ #{order.Id}";
            mailMessage.Body = "test msg";
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }
    }
}