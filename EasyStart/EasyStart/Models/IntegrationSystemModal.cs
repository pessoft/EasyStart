using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class IntegrationSystemModal
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public IntegrationSystemType Type { get; set; }
        public string Secret { get; set; }

    }
}