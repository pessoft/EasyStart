using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Models;

namespace EasyStart.Logic.Report
{
    public class CountOrderReport : CountBaseReport
    {
        public CountOrderReport(ReportFilter filter):base(filter)
        {}

        protected override Dictionary<DateTime, double> GetData()
        {
            return AnalyticsDataWrapper.GetCountOrder(filter.DateFrom, filter.DateTo, filter.BranchId);
        }
    }
}