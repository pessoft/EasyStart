using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyStart.Models.ProductOption
{
    public class AdditionalOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public List<AdditionOptionItem> Items { get; set; }
        
    }
}