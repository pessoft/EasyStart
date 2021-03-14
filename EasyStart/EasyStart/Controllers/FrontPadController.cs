using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyStart.Controllers
{
    /// <summary>
    /// Web hook controller
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FrontPadController : ApiController
    {
        [HttpPost]
        public FrontPadOrderStatus ChangeStatus([FromBody] FrontPadOrderStatus status)
        {
            return status;
        }
    }
}