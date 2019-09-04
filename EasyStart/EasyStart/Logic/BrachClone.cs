using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.Logic
{
    public class BrachClone
    {
        private int mainBrachId;
        private int newBrachId;
        private HttpServerUtilityBase server;

        public BrachClone(HttpServerUtilityBase server, int mainBrachId, int newBrachId)
        {
            this.server = server;
            this.mainBrachId = mainBrachId;
            this.newBrachId = newBrachId;
        }

        public void Clone()
        {
            CloneCategories();
        }

        private void CloneCategories()
        {
            var baseCategories = DataWrapper.GetCategories(mainBrachId);

            foreach (var category in baseCategories)
            {
                var newImageName = CloneImage(category.Image);
                var newCategory = category.Clone(newBrachId, newImageName);

                newCategory = DataWrapper.SaveCategoryWihoutChangeOrderNumber(newCategory);

                CloneProducts(category.Id, newCategory.Id);
            }

        }

        private void CloneProducts(int baseCategoryId, int newCategoryId)
        {
            var baseProducts = DataWrapper.GetProducts(baseCategoryId);
            var newProdcts = new List<ProductModel>();

            foreach (var product in baseProducts)
            {
                var newImageName = CloneImage(product.Image);
                var newProduct = product.Clone(newBrachId, newCategoryId, newImageName);

                newProdcts.Add(newProduct);
            }

            DataWrapper.SaveProductsWihoutChangeOrderNumber(newProdcts);
        }

        private string CloneImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "";

            string baseFilePath = server.MapPath(imagePath);
            string fileName = System.IO.Path.GetFileName(baseFilePath);
            string ext = fileName.Substring(fileName.LastIndexOf("."));
            string newFileName = String.Format(@"{0}{1}", System.Guid.NewGuid(), ext);
            string newPath = server.MapPath("~/Images/Products/" + newFileName);
            File.Copy(baseFilePath, newPath);

            return $"../Images/Products/{newFileName}";
        }

    }
}