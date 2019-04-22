using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class DeliverySettingModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public double PriceDelivery { get; set; }
        public double FreePriceDelivery { get; set; }
        public bool PayCard { get; set; }
        public bool PayCash { get; set; }
        public string TimeDeliveryJSON { get; set; }

        [NotMapped]
        public Dictionary<int, List<string>> TimeDelivery
        { get
            {
                if (!string.IsNullOrEmpty(TimeDeliveryJSON))
                {
                    return JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(TimeDeliveryJSON);
                }

                return null;
            }
        }
    }
}