using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Models;

namespace EasyStart.Logic.Report
{
    public class DeliveryMethodReport : BaseReport, IReport
    {

        public DeliveryMethodReport(ReportFilter filter) : base(filter)
        { }

        public ReportResult GetReport()
        {
            var result = new ReportResult();
            var data = AnalyticsDataWrapper.GetDeliveryMethodOrders(filter.DateFrom, filter.DateTo, filter.BranchId);
            int delivertCount = 0;
            var delivertCountPercent = 0;
            var takeYourSelfPercent = 0;

            data.TryGetValue(DeliveryType.Delivery, out delivertCount);
            data.TryGetValue(DeliveryType.TakeYourSelf, out int takeYourSelf);

            if (delivertCount == 0 && takeYourSelf != 0)
            {
                takeYourSelfPercent = 100;
            }
            else if (takeYourSelf == 0 && delivertCount != 0)
            {
                delivertCountPercent = 100;
            }
            else if(takeYourSelf != 0 && delivertCount != 0)
            {
                delivertCountPercent = (int)Math.Floor((delivertCount * 1.0 / (delivertCount + takeYourSelf) * 100.0));
                takeYourSelfPercent = 100 - delivertCountPercent;
            }

            result.Labels.Add("Доставка курьером");
            result.Data.Add(delivertCountPercent);
            result.Labels.Add("Самовывоз");
            result.Data.Add(takeYourSelfPercent);

            return result;
        }
    }
}