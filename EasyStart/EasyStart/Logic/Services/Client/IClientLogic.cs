using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientModel = EasyStart.Models.Client;

namespace EasyStart.Logic.Services.Client
{
    public interface IClientLogic
    {
        List<ClientModel> GetAll(int branchId);
        ClientModel Get(int id);
        double SetVirtualMoney(int clientId, double virtualMoney);
        void Block(int clientId);
        void UnBlock(int clientId);
        void SetVirtualMoney(Dictionary<string, double> data);
    }
}
