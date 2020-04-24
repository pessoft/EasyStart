using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EasyStart.Utils
{
    public static class Utils
    {
        public static long ConvertRubToKopeks(double rub)
        {
            long kopeks = Convert.ToInt64(rub * 100);

            return kopeks;
        }

        public static string SHA1(string str)
        {
            SHA1 shaCrypto = new SHA1CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(str);
            var hashBytes = shaCrypto.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.AppendFormat("{0:x2}", hashByte);
            }

            var hashString = sb.ToString();

            return hashString;
        }
    }
}