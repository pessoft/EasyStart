using System.Collections.Generic;
using ClientModel = EasyStart.Models.Client;

namespace EasyStart.Logic.Services.Client
{
    public interface IClientLogic: IBranchRemoval
    {
        List<ClientModel> GetAll(int branchId);
        ClientModel Get(int id);
        double SetVirtualMoney(int clientId, double virtualMoney);
        void Block(int clientId);
        void UnBlock(int clientId);
        void SetVirtualMoney(Dictionary<string, double> data);
    }
}
