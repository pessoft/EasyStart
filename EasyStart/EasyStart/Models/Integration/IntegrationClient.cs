using Ganss.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Models.Integration
{
    public class IntegrationClient
    {
        [Column("Телефон")]
        public string PhoneNumber { get; set; }

        [Column("Лицевой счет")]
        public double VirtualMoney { get; set; }
    }
}
