using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem.SendNewOrderResult
{
    public class FrontPadNewOrderResult
    {
        /// <summary>
        /// Строкое представление Success
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonIgnore]
        public bool Success
        {
            get
            {
                return Result == "success";
            }
        }

        [JsonProperty("order_id")]
        public long OrderId { get; set; }

        [JsonProperty("order_number")]
        public int OrderNumber { get; set; }

        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}