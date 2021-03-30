using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class IntegrationSystemModel: BaseEntity
    {
        public int BranchId { get; set; }
        public IntegrationSystemType Type { get; set; }
        public string Secret { get; set; }
        public bool UseAutomaticDispatch { get; set; }

        /// <summary>
        /// JSON string - different for integration systems 
        /// </summary>
        public string Options { get; set; }
    }
}