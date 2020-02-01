using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.HtmlRenderer
{
    public class EmailRestorePasswordBodyHtmlRenderer : IHtmlRenderer
    {
        private Client client;
        private string template;
        public EmailRestorePasswordBodyHtmlRenderer(Client client, string emailTemplate)
        {
            this.client = client;
            this.template = emailTemplate;
        }

        public string Render()
        {
            return template
                .Replace("{phoneNumber}", client.PhoneNumber)
                .Replace("{password}", client.Password);
        }
    }
}