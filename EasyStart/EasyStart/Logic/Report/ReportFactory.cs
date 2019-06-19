using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Report
{
    public static class ReportFactory
    {
        public static IReport GetInstance(ReportFilter filter)
        {
            switch (filter.ReportType)
            {
                case ReportType.CountOrder:
                    return new CountOrderReport(filter);
                case ReportType.Top5Categories:
                    return new TopCategoriesReport(filter);
                case ReportType.Top5Products:
                    return new TopProductsReport(filter);
                case ReportType.Revenue:
                    return new RevenueReport(filter);
                case ReportType.DeliveryMethod:
                    return new DeliveryMethodReport(filter);
                default:
                    throw new Exception("Unknown repot type");
            }
        }
    }
}