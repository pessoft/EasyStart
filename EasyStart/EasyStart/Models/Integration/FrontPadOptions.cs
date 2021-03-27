using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Integration
{
    public class FrontPadOptions
    {
        public int StatusNew { get; set; }
        public int StatusProcessed { get; set; }
        public int StatusDelivery { get; set; }
        public int StatusCancel { get; set; }
        public int StatusDone { get; set; }
        public string PhoneCodeCountry { get; set; }
        public bool UsePhoneMask { get; set; } = true;
    }
}