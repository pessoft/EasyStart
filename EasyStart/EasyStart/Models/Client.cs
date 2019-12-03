using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string ReferralCode { get; set; }
        public int ParentRefferalClientId { get; set; }
        public double VirtualMoney { get; set; }
        public double RefferalDiscount { get; set; }
    }
}