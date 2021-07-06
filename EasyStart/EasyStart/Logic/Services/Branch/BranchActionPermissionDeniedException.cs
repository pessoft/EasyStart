using System;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchActionPermissionDeniedException : Exception
    {
        public BranchActionPermissionDeniedException() : base("Нет прав для выполнения действия")
        { }
    }
}