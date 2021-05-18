using EasyStart.Logic.Services.Branch;
using EasyStart.Models;
using EasyStart.Repositories;
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

        public string SaveImage(HttpServerUtilityBase server, HttpRequestBase request)
        {
            return utilsLogic.SaveImage(server, request);
        }
    }
}