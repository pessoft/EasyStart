using EasyStart.HtmlRenderer;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EasyStart.Logic.Notification.EmailNotification
{
    public class OrderEmailNotification
    {
        private int orderNumber;
        private IEmail email;
        private string toEmail;

        public OrderEmailNotification(int orderNumber, IEmail email, List<string> toEmails)
        {
            this.orderNumber = orderNumber;
            this.email = email;
            this.toEmail = string.Join(",", toEmails);
        }

        public void EmailNotify(IHtmlRenderer bodyHtmlRenderer)
        {
            var mailMessage = getMailMessage(bodyHtmlRenderer);

            email.Send(mailMessage);
        }

        private MailMessage getMailMessage(IHtmlRenderer bodyHtmlRenderer)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = $"Заказ #{orderNumber}";
            mailMessage.Body = bodyHtmlRenderer.Render();
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }
    }
}