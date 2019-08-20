﻿using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string AdditionInfo { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public double Rating { get; set; }
        public double VotesSum { get; set; }
        public int VotesCount { get; set; }
        public ProductType ProductType { get; set; }
        public int OrderNumber { get; set; }
        public bool Visible { get; set; } = true;
    }
}