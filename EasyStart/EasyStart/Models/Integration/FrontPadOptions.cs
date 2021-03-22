﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Integration
{
    public class FrontPadOptions
    {
        public int StatusProcessed { get; set; }
        public int StatusDelivery { get; set; }
        public int StatusCancel { get; set; }
        public int StatusDone { get; set; }
    }
}