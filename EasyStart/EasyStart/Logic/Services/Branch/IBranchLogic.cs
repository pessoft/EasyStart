using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.Branch
{
    public interface IBranchLogic: IBranchRemoval
    {
        BranchModel Save(BranchModel branch);
        BranchModel Get();
        BranchModel Get(string login);
        BranchModel GetMainBranch();
    }
}
