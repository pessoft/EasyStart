using System;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchAlreadyExistException : Exception
    {
        public BranchAlreadyExistException() : base("Учетная запись с таким логином уже существует")
        { }
    }
}