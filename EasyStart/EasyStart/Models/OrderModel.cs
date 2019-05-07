using EasyStart.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public string Street { get; set; }
        public string HomeNumber { get; set; }
        public string EntranceNumber { get; set; }
        public string ApartamentNumber { get; set; }
        public string Level { get; set; }
        public string IntercomCode { get; set; }
        public BuyType BuyType { get; set; }
        public string Comment { get; set; }
        public string ProductCountJSON { get; set; }
        public double Discount { get; set; }
        public double CashBack { get; set; }
        public bool NeedCashBack { get; set; }

        [NotMapped]
        public Dictionary<int, int> ProductCount
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<int, int>>(ProductCountJSON);
            }
        }
    }
}