﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class JsonResultModel
    {
        public bool Success { get; set; }
        public string ErrorMEssage { get; set; }
        public string URL { get; set; }
    }
}