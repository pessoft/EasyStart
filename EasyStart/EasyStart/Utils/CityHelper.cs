using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EasyStart.Utils
{
    public static class CityHelper
    {
        public static Dictionary<int, string> City = new Dictionary<int, string>();
        static CityHelper()
        {
            try
            {
                var city = File.ReadAllText("~/Resource/City.json");
                City = JsonConvert.DeserializeObject<Dictionary<int, string>>(city);
            }
            catch (Exception ex)
            { }
        }
    }
}