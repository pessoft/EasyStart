using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class HistoryOrderFilter
    {
        public List<int> BranchIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}