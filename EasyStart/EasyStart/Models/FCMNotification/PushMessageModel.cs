using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace EasyStart.Models.FCMNotification
{
    public class PushMessageModel: IBaseEntity<int>
    {
        public PushMessageModel()
        {  }

        public PushMessageModel(FCMMessage message, int branchId, DateTime date)
        {
            BranchId = branchId;
            Title = message.Title;
            Body = message.Body;
            ImageUrl = message.ImageUrl;
            DataJSON = message.Data != null ?  JsonConvert.SerializeObject(message.Data) : null;
            Date = date;
        }

        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public string DataJSON { get; set;}
        public DateTime Date { get; set; }

        [NotMapped]
        [ScriptIgnore]
        public Dictionary<string, string> Data
        {
            get
            {
                if (string.IsNullOrEmpty(DataJSON))
                    return null;
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(DataJSON);
            }
        }
    }
}