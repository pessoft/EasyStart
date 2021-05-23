using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchActionPermissionDeniedException : Exception
    {
        public BranchActionPermissionDeniedException() : base("Нет прав для выполнения действия")
        { }
    }
}