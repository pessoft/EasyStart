using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class RatingProductMiddleware
    {
        public double Rating { get; set; }
        public double VotesSum { get; set; }
        public int VotesCount { get; set; }
    }
}