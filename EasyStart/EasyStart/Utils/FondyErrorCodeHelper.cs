using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EasyStart.Utils
{
    public static class FondyErrorCodeHelper
    {
        public static Dictionary<int, string> Errors = new Dictionary<int, string>();
        static FondyErrorCodeHelper()
        {
            try
            {
                var errors = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Resource/fondy-error-code-ru.json"));
                Errors = JsonConvert.DeserializeObject<Dictionary<int, string>>(errors);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static string GetErrorDescription(int id)
        {
            string description = "Неизвестная ошибка";

            Errors.TryGetValue(id, out description);

            return description;
        }
    }
}