using EasyStart.HtmlRenderer;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EasyStart.Logic.Notification.EmailNotification
{
    public class RestorePasswordNotification
    {
        private IEmailSender emailSender;
        private Client client;

        public RestorePasswordNotification(Client clinet, IEmailSender emailSender)
        {
            this.emailSender = emailSender;
            this.client = clinet;
        }

        public void EmailNotify(IHtmlRenderer bodyHtmlRenderer)
        {
            if (string.IsNullOrEmpty(client.Email))
            {
                return;
            }

            var mailMessage = getMailMessage(bodyHtmlRenderer);

            emailSender.Send(mailMessage);
        }

        private MailMessage getMailMessage(IHtmlRenderer bodyHtmlRenderer)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(client.Email);
            mailMessage.Subject ="Восстановление пароля";
            mailMessage.Body = bodyHtmlRenderer.Render();
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }
    }
}