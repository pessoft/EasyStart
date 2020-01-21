using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Utils
{
    public static class DateTimeHepler
    {
        private static Dictionary<string, TimeZoneInfo> timeZones;
        public const string DEFAULT_ZONE_ID = "Russian Standard Time";

        static DateTimeHepler()
        {
            timeZones = (new List<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones()))
                        .ToDictionary(p => p.Id, p => p);
        }

        public static DateTime GetDateTimeNow(this DateTime dateTime, string zoneId)
        {
            if(!string.IsNullOrEmpty(zoneId))
            {
                var zone = GetZone(zoneId);
                var newDateTime = TimeZoneInfo.ConvertTime(dateTime, zone);

                return newDateTime;
            } else
            {
                return dateTime;
            }
        }

        public static Dictionary<string, string> GetDisplayDictionary()
        {
            return timeZones.ToDictionary(p => p.Key, p => p.Value.DisplayName);
        }

        public static TimeZoneInfo GetZone(string zoneId)
        {
            TimeZoneInfo zone;

            if (timeZones.TryGetValue(zoneId, out zone))
            {
                return zone;
            }

            return timeZones[DEFAULT_ZONE_ID];
        }
    }
}