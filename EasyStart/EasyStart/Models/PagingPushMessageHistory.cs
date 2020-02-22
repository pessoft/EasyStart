using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class PagingPushMessageHistory
    {
        public object HistoryMessages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsLast { get; set; }
    }
}