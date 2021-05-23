using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchAlreadyExistException : Exception
    {
        public BranchAlreadyExistException() : base("Учетная запись с таким логином уже существует")
        { }
    }
}