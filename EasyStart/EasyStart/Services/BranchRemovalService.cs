using EasyStart.Logic.Services;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.ConstructorProduct;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.GeneralSettings;
using EasyStart.Logic.Services.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class BranchRemovalService
    {
        private List<IBranchRemoval> logic;
        private IBranchLogic branchLogic;
        public BranchRemovalService(
            IBranchLogic branchLogic,
            IGeneralSettingsLogic generalSettingsLogic,
            IDeliverySettingLogic deliverySettingLogic,
            IClientLogic clientLogic,
            ICategoryProductLogic categoryProductLogic,
            IProductLogic productLogic,
            IConstructorProductLogic constructorProductLogic)
        {
            this.branchLogic = branchLogic;
            logic = new List<IBranchRemoval>
            { 
                branchLogic,
                generalSettingsLogic,
                deliverySettingLogic,
                clientLogic,
                categoryProductLogic,
                productLogic,
                constructorProductLogic
            };
        }

        public void Remove(int branchId)
        {
            var branch = branchLogic.Get();

            if (branch.Id == branchId)
                throw new BranchSelfDeletionException();

            if (branch.TypeBranch != Logic.TypeBranch.MainBranch)
                throw new BranchActionPermissionDeniedException();

            foreach (var itemLogic in logic)
            {
                if (itemLogic != null)
                    itemLogic.RemoveByBranch(branchId);
            }
        }
    }
}