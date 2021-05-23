using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchSelfDeletionException : Exception
    {
        public BranchSelfDeletionException() : base("Самоудаление не возможно")
        { }
    }
}