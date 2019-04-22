using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Utils
{
    public static class WeeklyDayHelper
    {
        private static Dictionary<DayWeekly, string> dayDictionary;

        static WeeklyDayHelper()
        {
            dayDictionary = new Dictionary<DayWeekly, string>()
            {
                { DayWeekly.Monday, "Понедельник"},
                { DayWeekly.Tuesday, "Вторник"},
                { DayWeekly.Wednesday, "Среда"},
                { DayWeekly.Thursday, "Четверг"},
                { DayWeekly.Friday, "Пятница"},
                { DayWeekly.Saturday, "Суббота"},
                { DayWeekly.Sunday, "Воскресенье"}
            };
        }

        public static string GetDay(DayWeekly dayWeekly)
        {
            var dayRus = "";

            if(!dayDictionary.TryGetValue(dayWeekly, out dayRus))
            {
                throw new Exception($"{dayWeekly.ToString()} отсутствует в словаре");
            }

            return dayRus;
        }

        public static List<Tuple<string, string, string, string, bool>> ConvertTimeDeliveryToViev(Dictionary<int, List<string>> timeDelivery = null)
        {
            if (timeDelivery == null)
            {
                return GetEmptyDeliveryToViev();
            }

            var days = new List<Tuple<string, string, string, string, bool>>();

            foreach (var pair in timeDelivery)
            {
                var dayEn = ((DayWeekly)pair.Key).ToString();
                var deyRus = WeeklyDayHelper.GetDay((DayWeekly)pair.Key);
                var startTime = "";
                var endTime = "";
                var checkDay = false;

                if (pair.Value != null && pair.Value.Count == 2)
                {
                    startTime = pair.Value[0];
                    endTime = pair.Value[1];
                    checkDay = true;
                }

                var dayTuple = new Tuple<string, string, string, string, bool>(deyRus, dayEn, startTime, endTime, checkDay);
                days.Add(dayTuple);
            }

            return days;
        }

        public static List<Tuple<string, string, string, string, bool>> GetEmptyDeliveryToViev()
        {
            var days = new List<Tuple<string, string, string, string, bool>>();

            foreach (var pair in dayDictionary)
            {
                var dayEn = pair.Key.ToString();
                var deyRus = pair.Value;
                var startTime = "";
                var endTime = "";
                var checkDay = false;

                var dayTuple = new Tuple<string, string, string, string, bool>(deyRus, dayEn, startTime, endTime, checkDay);
                days.Add(dayTuple);
            }

            return days;
        }
    }
}