using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class SettingsValidator
    {
        public bool IsValidSetting(SettingModel setting)
        {
            if (setting == null)
                return false;

            if (string.IsNullOrEmpty(setting.City))
                return false;

            if (string.IsNullOrEmpty(setting.Street))
                return false;

            if (setting.HomeNumber < 0)
                return false;

            if (string.IsNullOrEmpty(setting.PhoneNumber))
                return false;

            return true;
        }

        public bool IsValidDeliverySetting(DeliverySettingModel setting)
        {
            if (setting == null)
                return false;

            if (setting.TimeDelivery == null || !setting.TimeDelivery.Any())
                return false;

            if (setting.AreaDeliveries == null || !setting.AreaDeliveries.Any())
                return false;

            return true;
        }
    }
}