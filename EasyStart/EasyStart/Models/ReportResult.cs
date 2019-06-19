using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ReportResult
    {
        public ReportResult()
        {
            Data = new List<double>();
            Labels = new List<string>();
        }

        public List<double> Data { get; set; }
        public List<string> Labels { get; set; }
    }
}