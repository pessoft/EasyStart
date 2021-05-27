using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class Client: IBaseEntity<int>
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateBirth { get; set; }
        public string ReferralCode { get; set; }
        public int ParentReferralClientId { get; set; }
        public string ParentReferralCode { get; set; }
        public double VirtualMoney { get; set; }
        public double ReferralDiscount { get; set; }
        public int CityId { get; set; }
        public int BranchId { get; set; }
        public bool Blocked { get; set; }
    }
}