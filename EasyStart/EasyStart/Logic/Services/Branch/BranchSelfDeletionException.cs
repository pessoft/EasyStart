using System;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchSelfDeletionException : Exception
    {
        public BranchSelfDeletionException() : base("Самоудаление не возможно")
        { }
    }
}