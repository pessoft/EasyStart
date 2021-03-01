using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public abstract class BaseIntegrationSystem : IIntegrationSystem
    {
        protected readonly IntegrationSystemModel integrationSystemSetting;
        
        public BaseIntegrationSystem(IntegrationSystemModel integrationSystemSetting)
        {
            this.integrationSystemSetting = integrationSystemSetting;
        }

        public abstract INewOrderResult SendOrder(IOrderDetails orderDetails);

        protected async Task<string> SendOrder(string url, string postData)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = await request.GetResponseAsync();
            string responseResult = null;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseResult =  reader.ReadToEnd();
                }
            }

            response.Close();

            return responseResult;
        }
    }
}