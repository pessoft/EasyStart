using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Utils
{
    public class ServerUtility : IServerUtility
    {
        private readonly HttpServerUtilityBase httpServerUtility;
        public ServerUtility(HttpServerUtilityBase httpServerUtility)
        {
            this.httpServerUtility = httpServerUtility;
        }
        public string MapPath(string path)
        {
            return this.httpServerUtility.MapPath(path);
        }
    }
}