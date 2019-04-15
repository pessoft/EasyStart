using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class BranchModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int CityId { get; set; }
    }
}