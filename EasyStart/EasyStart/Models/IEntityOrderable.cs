using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Models
{
    public interface IEntityOrderable<T>: IBaseEntity<T>
    {
        int OrderNumber { get; set; }
    }
}
