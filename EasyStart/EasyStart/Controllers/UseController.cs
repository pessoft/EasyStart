using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyStart.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UseController : ApiController
    {
        [HttpGet]
        public void Access(string id)
        {
            UseMethod.Use(id);
        }
    }
}