using EasyStart.Logic;
using EasyStart.Logic.AdditionalOptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductModel : BaseEntity<int>, IContainImage
    {
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
        public bool IsDeleted { get; set; }
        public string VendorCode { get; set; }
        public ProductAdditionalInfoType ProductAdditionalInfoType { get; set; }

        [NotMapped]
        public List<int> ProductAdditionalOptionIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> ProductAdditionalFillingIds { get; set; } = new List<int>();

        public string AllowCombinationsJSON { get; set; }

        public string AllowCombinationsVendorCodeJSON { get; set; }

        [NotMapped]
        public List<List<int>> AllowCombinations
        {
            get
            {
                if (!string.IsNullOrEmpty(AllowCombinationsJSON))
                    return JsonConvert.DeserializeObject<List<List<int>>>(AllowCombinationsJSON);
                else
                    return null;
            }
        }

        [NotMapped]
        public Dictionary<string, List<int>> AllowCombinationsVendorCode
        {
            get
            {
                if (!string.IsNullOrEmpty(AllowCombinationsVendorCodeJSON))
                    return JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(AllowCombinationsVendorCodeJSON);
                else
                    return null;
            }
        }
    }
}