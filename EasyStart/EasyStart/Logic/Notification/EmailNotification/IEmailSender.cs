using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EasyStart.Logic.Notification.EmailNotification
{
    public interface IEmailSender
    {
        void Send(MailMessage mailMessage);
    }
}