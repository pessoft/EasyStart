using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.OnlinePay.Fondy
{
    public class PaymentStatus
    {
        [JsonProperty("response_status")]
        public string ResponseStatus { get; set; }

        [JsonProperty("actual_amount")]
        public long ActualAmound { get; set; }

        [JsonProperty("order_status")]
        public string OrderStatus { get; set; }

        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }
    }
}