using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.IntegrationSystem
{
    public interface IIntegrationSystem
    {
        bool SendOrder(IOrderDetails orderDetails);
    }
}
