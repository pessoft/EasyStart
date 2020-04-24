using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Report
{
    public class ActiveUsersReport : BaseReport, IReport
    {
        public ActiveUsersReport(ReportFilter filter) : base(filter)
        { }

        protected  Dictionary<DateTime, List<int>> GetData()
        {
            return AnalyticsDataWrapper.GetActiveUsers(filter.DateFrom, filter.DateTo);
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

            if (data != null && data.Any())
            {
                dates?.ForEach(p =>
                {
                    if (!data.ContainsKey(p))
                    {
                        data.Add(p, new List<int>());
                    }
                });

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
                        setResult(group.Key.ToString(), group.SelectMany(s => s.Value).Distinct().Count());
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
                        setResult(group.Key.ToString("MM/yy"), group.SelectMany(s => s.Value).Distinct().Count());
                    }
                }
                else
                {
                    foreach (var pk in data.OrderBy(p => p.Key))
                    {
                        setResult(pk.Key.ToString("dd/MM/yy"), pk.Value.Count());
                    }
                }
            }

            return result;
        }
    }
}