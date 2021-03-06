﻿using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class BranchSettingViewModel
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Addres { get; set; }
        public string OperationMode { get; set; }
        public string PhoneNumber { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}