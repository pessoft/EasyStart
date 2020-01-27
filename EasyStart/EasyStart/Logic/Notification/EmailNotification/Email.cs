using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EasyStart.Logic.Notification.EmailNotification
{
    public class Email : IEmail
    {
        public void Send(MailMessage mailMessage)
        {
            try
            {
                var client = new SmtpClient();
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }
    }
}