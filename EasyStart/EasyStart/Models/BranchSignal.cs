using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class BranchSignal
    {
        public string ConnectionId { get; set; }
        public List<int> BranchIds{ get; set; }
    }
}