using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class UpdaterOrderStatus
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime? ApproximateDeliveryTime { get; set; }
        public string CommentCauseCancel { get; set; }
    }
}