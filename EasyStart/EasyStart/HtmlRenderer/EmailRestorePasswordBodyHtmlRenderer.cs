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

        public EmailRestorePasswordBodyHtmlRenderer(Client client, string emailTemplate)
        {
            this.client = client;
        }

        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}