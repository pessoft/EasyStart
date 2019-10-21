using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EasyStart.Logic.EmailNotification
{
    public class Email : IEmail
    {
        private string login;
        private string password;
        public Email(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        public void Send(MailMessage mailMessage)
        {
            var client = new SmtpClient();
            client.Send(mailMessage);
        }
    }
}