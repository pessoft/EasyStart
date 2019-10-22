using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EasyStart.Logic.EmailNotification
{
    public interface IEmail
    {
        void Send(MailMessage mailMessage);
    }
}