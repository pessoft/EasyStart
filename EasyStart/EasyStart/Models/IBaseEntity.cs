using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public interface IBaseEntity<T>
    {
        [Key]
        T Id { get; set; }
    }
}