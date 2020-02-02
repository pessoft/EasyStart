using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class SettingModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int CityId { get; set; }
        public string Street { get; set; }
        public int HomeNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberAdditional { get; set; }
        public string Email { get; set; }
        public string Vkontakte { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string City
        {
            get
            {
                if (CityId != 0)
                {
                    return CityHelper.GetCity(CityId);
                }

                return "";
            }
        }
    }
}