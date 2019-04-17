﻿using EasyStart.Utils;
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
        public double PriceDelivery { get; set; }
        public double FreePriceDelivery { get; set; }
        public double TimeOpen { get; set; }
        public double TimeClose { get; set; }
        public string PhoneNumber { get; set; }

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