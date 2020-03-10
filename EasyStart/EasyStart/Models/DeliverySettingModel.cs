using EasyStart.Utils;
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
        public bool PayCard { get; set; }
        public bool PayCash { get; set; }

        public bool IsSoundNotify { get; set; }

        public string NotificationEmail { get; set; }

        public bool IsDeleted { get; set; }

        public int MaxPreorderPeriod { get; set; }

        /// <summary>
        /// Format hh:mm
        /// </summary>
        public string MinTimeProcessingOrder { get; set; } = "01:00";

        /// <summary>
        /// Time Zone Id
        /// </summary>
        public string ZoneId { get; set; } = DateTimeHepler.DEFAULT_ZONE_ID;
        public string TimeDeliveryJSON { get; set; }

        [NotMapped]
        public List<AreaDeliveryModel> AreaDeliveries { get; set; }

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