using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Report
{
    public class RevenueReport: CountBaseReport
    {
        public RevenueReport(ReportFilter filter):base(filter)
        { }

        protected override Dictionary<DateTime, double> GetData()
        {
            return AnalyticsDataWrapper.GetRevenue(filter.DateFrom, filter.DateTo, filter.BranchId);
        }

        protected override double RecalcValue<K>(IGrouping<K, KeyValuePair<DateTime, double>> group)
        {
            return group.Sum(p => p.Value);
        }
    }
}