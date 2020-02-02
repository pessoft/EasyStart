using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Utils
{
    public static class StringHelper
    {
        public static string CloneOrDefault(this string str)
        {
            return str != null ? string.Copy(str) : default(string);
        }
    }
}