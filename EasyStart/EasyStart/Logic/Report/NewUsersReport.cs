using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Report
{
    public class NewUsersReport: CountBaseReport
    {
        public NewUsersReport(ReportFilter filter) : base(filter)
        { }

        protected override Dictionary<DateTime, double> GetData()
        {
            return AnalyticsDataWrapper.GetNewUsers(filter.DateFrom, filter.DateTo);
        }
    }
}