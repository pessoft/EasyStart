using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.Branch
{
    public interface IBranchLogic: IBranchRemoval
    {
        BranchModel Save(BranchModel branch);
        BranchModel Get();
        BranchModel Get(string login);
        BranchModel GetMainBranch();
        List<BranchSettingViewModel> GetBranches(IEnumerable<SettingModel> generalSettings);
    }
}
