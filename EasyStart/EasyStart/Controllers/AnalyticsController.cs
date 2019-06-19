using EasyStart.Logic.Report;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.Controllers
{
    public class AnalyticsController : Controller
    {
        [HttpPost]
        [Authorize]
        public JsonResult GetReport(ReportFilter filter)
        {
            var result = new JsonResultModel();

            filter.DateFrom = filter.DateFrom.Date;
            filter.DateTo = filter.DateTo.Date;
            var report = ReportFactory.GetInstance(filter);
            var data = report.GetReport();

            result.Data = data;
            result.Success = true;

            if (data == null ||
                data.Data == null ||
                !data.Data.Any())
            {
                result.ErrorMessage = "Report is empty";
                result.Success = false;
            }

            return Json(result);
        }
    }
}