using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Models;

namespace EasyStart.Logic.Report
{
    public class CountBaseReport : BaseReport, IReport
    {
        public CountBaseReport(ReportFilter filter): base(filter)
        { }

        protected virtual Dictionary<DateTime, double> GetData()
        {
            throw new Exception("Not implemented method");
        }

        public ReportResult GetReport()
        {
            ReportResult result = null;
            var data = GetData();
            var dateDiff = ((filter.DateTo.Year - filter.DateFrom.Year) * 12) + filter.DateTo.Month - filter.DateFrom.Month;
            var countDays = (filter.DateTo - filter.DateFrom).Days + 1;
            var dates = Enumerable
                        .Range(0, countDays)
                        .Select(day => filter.DateFrom.AddDays(day))
                        .ToList();

            dates?.ForEach(p =>
            {
                if (!data.ContainsKey(p))
                {
                    data.Add(p, 0);
                }
            });

            if (data != null && data.Any())
            {
                result = new ReportResult();
                Action<string, double> setResult = (label, value) =>
                {
                    result.Labels.Add(label);
                    result.Data.Add(value);
                };

                if (dateDiff > 12)
                {

                    foreach (var group in data.GroupBy(p => p.Key.Year).OrderBy(p => p.Key))
                    {
                        setResult(group.Key.ToString(), group.Sum(s => s.Value));
                    }
                }
                else if (dateDiff > 0 && countDays > 28)
                {
                    foreach (var group in data
                                            .GroupBy(p => new DateTime(p.Key.Year,
                                                                       p.Key.Month,
                                                                       DateTime.DaysInMonth(p.Key.Year, p.Key.Month)))
                                            .OrderBy(p => p.Key))
                    {
                        setResult(group.Key.ToString("MM/yy"), group.Sum(s => s.Value));
                    }
                }
                else
                {
                    foreach (var pk in data.OrderBy(p => p.Key))
                    {
                        setResult(pk.Key.ToString("dd/MM/yy"), pk.Value);
                    }
                }
            }

            return result;
        }
    }
}