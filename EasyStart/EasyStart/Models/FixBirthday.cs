using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class FixBirthday
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime DateUse { get; set; }
        public DateTime DateBirth { get; set; }
    }
}