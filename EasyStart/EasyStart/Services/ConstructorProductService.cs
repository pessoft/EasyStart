using EasyStart.Logic.Services.ConstructorProduct;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class ConstructorProductService
    {
        private readonly IConstructorProductLogic constructorProductLogic;

        public ConstructorProductService(IConstructorProductLogic constructorProductLogic)
        {
            this.constructorProductLogic = constructorProductLogic;
        }

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            constructorProductLogic.UpdateOrder(items);
        }
    }
}