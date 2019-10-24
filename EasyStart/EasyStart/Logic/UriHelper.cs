using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public static class UriHelper
    {
        public static string GetBaseUrl(this Uri uri)
        {
            return uri == null ?
                null :
                string.Format("{0}://{1}", uri.Scheme, uri.Authority);
        }
    }
}