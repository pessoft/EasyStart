using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.Product
{
    public interface ICategoryProductLogic
    {
        CategoryModel Get(int id);
        CategoryModel SaveCategory(CategoryModel category);
        bool RemoveCategory(int id);
    }
}
