using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class TodayDataOrdersModel
    {
        public int CountSuccesOrder { get; set; }
        public int CountCancelOrder { get; set; }
        public double Revenue { get; set; }
    }
}