using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Integration
{
    public class FrontPadOrderStatus
    {
        public string Action { get; set; }

        /// <summary>
        /// External order id
        /// </summary>
        [JsonProperty(PropertyName = "order_id")]
        public long OrderId { get; set; }

        public int Status { get; set; }

        public DateTime Datetime { get; set; }
    }
}