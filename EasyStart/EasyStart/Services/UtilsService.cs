using EasyStart.Logic.Services.Branch;
using EasyStart.Models;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class UtilsService
    {
        private readonly IUtilsLogic utilsLogic;

        public UtilsService(IUtilsLogic utilsLogic)
        {
            this.utilsLogic = utilsLogic;
        }

        public string SaveImage(HttpRequestBase request)
        {
            return utilsLogic.SaveImage(request);
        }
    }
}