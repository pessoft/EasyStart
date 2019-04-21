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

    }
}