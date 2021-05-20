using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class UniqIdEntity
    {
        [Key]
        public string Id { get; set; }
    }
}