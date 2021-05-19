using EasyStart.Logic.Services.Utils;
using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Product
{
    public class CategoryProductLogic : ICategoryProductLogic
    {
        private readonly IDefaultEntityRepository<CategoryModel> categoryRepository;
        private readonly IServerUtility serverUtility;

        public CategoryProductLogic(
            IDefaultEntityRepository<CategoryModel> categoryRepository,
            IServerUtility serverUtility)
        {
            this.categoryRepository = categoryRepository;
            this.serverUtility = serverUtility;
        }

        public CategoryModel Get(int id)
        {
            return categoryRepository.Get(id);
        }

        public CategoryModel SaveCategory(CategoryModel category)
        {
            var defaultImage = "../Images/default-image.jpg";
            var oldCategory = categoryRepository.Get(category.Id);
            CategoryModel savedCategory = null;

            if (!System.IO.File.Exists(serverUtility.MapPath(category.Image)))
                category.Image = defaultImage;

            if (oldCategory != null)
            {
                var oldImage = oldCategory.Image;

                if (oldImage != category.Image
                    && oldImage != defaultImage
                   && System.IO.File.Exists(serverUtility.MapPath(oldImage)))
                {
                    System.IO.File.Delete(serverUtility.MapPath(oldImage));
                }

                savedCategory = categoryRepository.Update(category);
            }
            else
                savedCategory = categoryRepository.Create(category);

            return savedCategory;
        }
    }
}