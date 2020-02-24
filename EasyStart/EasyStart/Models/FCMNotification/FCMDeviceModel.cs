﻿using EasyStart.Logic.FCM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.FCMNotification
{
    public class FCMDeviceModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ClientId { get; set; }
        public string Token { get; set;}
        public PlatformDevice Platform { get; set; }
    }
}