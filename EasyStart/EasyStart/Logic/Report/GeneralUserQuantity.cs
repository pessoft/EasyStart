using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Report
{
    public class GeneralUserQuantity : BaseReport, IReport
    {
        public GeneralUserQuantity(ReportFilter filter) : base(filter)
        { }

        protected int GetData()
        {
            return AnalyticsDataWrapper.GetGeneralUserQuantity(filter.BranchId);
        }

        public ReportResult GetReport()
        {
            var result = new ReportResult
            {
                Data = new List<double> { (double)GetData()}
            };
          
            return result;
        }
    }
}