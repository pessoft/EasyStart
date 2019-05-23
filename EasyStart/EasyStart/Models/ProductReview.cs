using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string PhoneNumber { get; set; }
        public string ReviewText { get; set; }
        public DateTime Date { get; set; }
        public bool Visible { get; set; } = true;
    }
}