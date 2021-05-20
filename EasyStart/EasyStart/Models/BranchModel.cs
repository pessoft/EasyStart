using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class BranchModel: BaseEntity<int>
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public TypeBranch TypeBranch { get; set; }
        public bool IsDeleted { get; set; }
    }
}