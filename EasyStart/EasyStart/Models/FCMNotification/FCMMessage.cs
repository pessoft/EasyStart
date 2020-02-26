using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.FCMNotification
{
    public class FCMMessage
    {
        public FCMMessage()
        {  }

        public FCMMessage(PushNotification pushNotification)
        {
            Title = pushNotification.Title;
            Body = pushNotification.Body;
            ImageUrl = pushNotification.ImageUrl;

            var payload = new
            {
                title = Title,
                message = Body,
                action = new
                {
                    type = pushNotification.Action.Type,
                    targetId = pushNotification.Action.TargetId
                }
            };

            Data = new Dictionary<string, string>() { { "payload", JsonConvert.SerializeObject(payload) } };
        }

        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}