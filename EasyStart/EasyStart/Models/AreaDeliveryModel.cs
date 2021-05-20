using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class AreaDeliveryModel: BaseEntity<string>
    {
        public int DeliverySettingId { get; set; }
        public string NameArea { get; set; }
        public double MinPrice { get; set; }
        public double DeliveryPrice { get; set; }
        public string VendorCode { get; set; }
    }
}