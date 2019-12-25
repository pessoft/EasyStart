using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class CouponParamsModel
    {
        public int ClientId { get; set; }
        public int BranchId { get; set; }
        public string Promocode { get; set; }
    }
}