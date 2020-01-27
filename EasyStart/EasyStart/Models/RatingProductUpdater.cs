using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class RatingProductUpdater
    {
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public double Score { get; set; }
    }
}