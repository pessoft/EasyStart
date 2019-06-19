using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Models;

namespace EasyStart.Logic.Report
{
    public class BaseReport
    {
        protected ReportFilter filter;
        protected BaseReport(ReportFilter filter)
        {
            this.filter = filter;
        }
    }
}