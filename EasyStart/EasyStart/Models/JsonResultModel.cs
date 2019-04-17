using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class JsonResultModel
    {
        public object Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string URL { get; set; }
    }
}