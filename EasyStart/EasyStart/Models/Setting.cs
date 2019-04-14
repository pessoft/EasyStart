using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string Street { get; set; }
        public int HomeNumber { get; set; }
        public double PriceDelivery { get; set; }
        public double FreePriceDelivery { get; set; }
        public double TimeOpen { get; set; }
        public double TimeClose { get; set; }
    }
}