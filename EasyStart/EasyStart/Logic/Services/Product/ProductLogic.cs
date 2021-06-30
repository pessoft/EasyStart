using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Product
{
    public class ProductLogic: IProductLogic
    {
        private readonly IBaseRepository<ProductModel, int> productRepository;
        private readonly IBaseRepository<AdditionalFilling, int> additionalFillingRepository;
        private readonly IBaseRepository<AdditionalOption, int> additionalOptionRepository;
        private readonly IBaseRepository<AdditionOptionItem, int> additionOptionItemRepository;
        private readonly IBaseRepository<ProductAdditionalFillingModal, int> productAdditionalFillingRepository;
        private readonly IBaseRepository<ProductAdditionalOptionModal, int> productAdditionOptionItemRepository;
        private readonly IContainImageLogic imageLogic;
        private readonly IDisplayItemSettingLogic displayItemSettingLogic;

        public ProductLogic(
            IBaseRepository<ProductModel, int> productRepository,
            IBaseRepository<AdditionalFilling, int> additionalFillingRepository,
            IBaseRepository<AdditionalOption, int> additionalOptionRepository,
            IBaseRepository<AdditionOptionItem, int> additionOptionItemRepository,
            IBaseRepository<ProductAdditionalFillingModal, int> productAdditionalFillingRepository,
            IBaseRepository<ProductAdditionalOptionModal, int> productAdditionOptionItemRepository,
            IContainImageLogic imageLogic,
            IDisplayItemSettingLogic displayItemSettingLogic)
        {
            this.productRepository = productRepository;
            this.additionalFillingRepository = additionalFillingRepository;
            this.additionalOptionRepository = additionalOptionRepository;
            this.additionOptionItemRepository = additionOptionItemRepository;
            this.productAdditionalFillingRepository = productAdditionalFillingRepository;
            this.productAdditionOptionItemRepository = productAdditionOptionItemRepository;
            this.imageLogic = imageLogic;
            this.displayItemSettingLogic = displayItemSettingLogic;
        }

        public ProductModel Get(int id)
        {
            var product = productRepository.Get(id);
            AppendProductAdditionalItems(new List<ProductModel> { product });

            return product;
        }

        public IEnumerable<ProductModel> Get(IEnumerable<int> ids)
        {
            if (ids == null)
                return new List<ProductModel>();

            var products = productRepository.Get(p => ids.Contains(p.Id)).ToList();
            AppendProductAdditionalItems(products);

            return products;
        }

        /// <summary>
        /// Возвращает продукты на основе ProductCount и ProductBonusCount
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<ProductModel> Get(OrderModel order)
        {
            var productIds = new List<int>();

            if (order.ProductCount != null)
                productIds.AddRange(order.ProductCount.Keys);

            if (order.ProductBonusCount != null)
                productIds.AddRange(order.ProductBonusCount.Keys);

            if (order.ProductWithOptionsCount != null)
                productIds.AddRange(order.ProductWithOptionsCount.Select(p => p.ProductId));

            productIds = productIds.Distinct().ToList();
            var products = productRepository.Get(p => productIds.Contains(p.Id)).ToList();

            AppendProductAdditionalItems(products);
            
            return products;
        }

        public List<AdditionalFilling> GetAdditionalFillingsByBranchId(int branchId)
        {
            var additionalfillings = additionalFillingRepository
                .Get(p => p.BranchId == branchId && !p.IsDeleted)
                .ToList();

            return additionalfillings;
        }

        public List<AdditionOptionItem> GetAdditionOptionItemByBranchId(int branchId)
        {
            var additionOptionItems = additionOptionItemRepository
                .Get(p => p.BranchId == branchId && !p.IsDeleted)
                .ToList();

            return additionOptionItems;
        }

        public Dictionary<int, List<ProductAdditionalFillingModal>> GetProductAdditionalFillingsByProductIds(List<int> productIds)
        {
            var additionalfillings = productAdditionalFillingRepository
                .Get(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .GroupBy(p => p.ProductId)
                .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var productId in productIds)
            {
                if (!additionalfillings.TryGetValue(productId, out List<ProductAdditionalFillingModal> fillings))
                    additionalfillings.Add(productId, new List<ProductAdditionalFillingModal>());
            }

            return additionalfillings;
        }

        public Dictionary<int, List<ProductAdditionalOptionModal>> GetProductAdditionOptionItemByProductIds(List<int> productIds)
        {
            var additionOptionItems = productAdditionOptionItemRepository
                .Get(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .GroupBy(p => p.ProductId)
                .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var productId in productIds)
            {
                if (!additionOptionItems.TryGetValue(productId, out List<ProductAdditionalOptionModal> options))
                    additionOptionItems.Add(productId, new List<ProductAdditionalOptionModal>());
            }

            return additionOptionItems;
        }

        public ProductModel SaveProduct(ProductModel product)
        {
            var oldProduct = productRepository.Get(product.Id);
            ProductModel savedProduct = null;

            imageLogic.PrepareImage(product);

            if(oldProduct != null)
            {
                product.OrderNumber = oldProduct.OrderNumber;

                imageLogic.RemoveOldImage(oldProduct, product);
                savedProduct = productRepository.Update(product);
            }
            else
            {
                var orderNumber = productRepository.Get(p => p.CategoryId == product.CategoryId && !p.IsDeleted).Count() + 1;
                product.OrderNumber = orderNumber;
                
                savedProduct = productRepository.Create(product);
            }

            savedProduct.ProductAdditionalOptionIds = product.ProductAdditionalOptionIds;
            SaveProductAdditionalOptions(savedProduct);

            savedProduct.ProductAdditionalFillingIds = product.ProductAdditionalFillingIds;
            SaveProductAdditionalFilling(savedProduct);

            return savedProduct;
        }

        private void SaveProductAdditionalOptions(ProductModel product)
        {
            var orderNumber = 1;
            var additionalOptions = product.ProductAdditionalOptionIds == null ? null : product.ProductAdditionalOptionIds
                .Select(p => new ProductAdditionalOptionModal
                {
                    AdditionalOptionId = p,
                    BranchId = product.BranchId,
                    OrderNumber = orderNumber++,
                    ProductId = product.Id
                }).ToList();
            var oldItems = productAdditionOptionItemRepository
                .Get(p => p.ProductId == product.Id && !p.IsDeleted)
                .ToList();
            oldItems.ForEach(p => p.IsDeleted = true);

            productAdditionOptionItemRepository.Update(oldItems);

            if(additionalOptions != null && additionalOptions.Any())
                productAdditionOptionItemRepository.Create(additionalOptions);
        }

        private void SaveProductAdditionalFilling(ProductModel product)
        {
            var orderNumber = 1;
            var additionalFillings = product.ProductAdditionalFillingIds == null ? null : product.ProductAdditionalFillingIds
                .Select(p => new ProductAdditionalFillingModal
                {
                    AdditionalFillingId = p,
                    BranchId = product.BranchId,
                    OrderNumber = orderNumber++,
                    ProductId = product.Id
                })
                .ToList();

            var oldItems = productAdditionalFillingRepository
                .Get(p => p.ProductId == product.Id && !p.IsDeleted)
                .ToList();
            oldItems.ForEach(p => p.IsDeleted = true);

            productAdditionalFillingRepository.Update(oldItems);

            if (additionalFillings != null && additionalFillings.Any())
                productAdditionalFillingRepository.Create(additionalFillings);
        }

        public void RemoveByBranch(int branchId)
        {
            RemoveProductsByBranch(branchId);
            RemoveAdditionalFillingsByBranch(branchId);
            RemoveAdditionOptionsByBranch(branchId);
            RemoveAdditionOptionItemsByBranch(branchId);
            RemoveProductAdditionalFillingsByBranch(branchId);
            RemoveProductAdditionOptionsByBranch(branchId);
        }

        private void RemoveProductsByBranch(int branchId)
        {
            var items = productRepository.Get(p => p.BranchId == branchId).ToList();
            items.ForEach(p => p.IsDeleted = true);
            productRepository.Update(items);
        }

        private void RemoveAdditionalFillingsByBranch(int branchId)
        {
            var items = additionalFillingRepository.Get(p => p.BranchId == branchId).ToList();
            items.ForEach(p => p.IsDeleted = true);
            additionalFillingRepository.Update(items);
        }

        private void RemoveAdditionOptionsByBranch(int branchId)
        {
            var items = additionalOptionRepository.Get(p => p.BranchId == branchId).ToList();
            items.ForEach(p => p.IsDeleted = true);
            additionalOptionRepository.Update(items);
        }

        private void RemoveAdditionOptionItemsByBranch(int branchId)
        {
            var items = additionOptionItemRepository.Get(p => p.BranchId == branchId).ToList();
            items.ForEach(p => p.IsDeleted = true);
            additionOptionItemRepository.Update(items);
        }

        private void RemoveProductAdditionalFillingsByBranch(int branchId)
        {
            var items = productAdditionalFillingRepository.Get(p => p.BranchId == branchId).ToList();
            items.ForEach(p => p.IsDeleted = true);
            productAdditionalFillingRepository.Update(items);
        }

        private void RemoveProductAdditionOptionsByBranch(int branchId)
        {
            var items = productAdditionOptionItemRepository.Get(p => p.BranchId == branchId).ToList();
            items.ForEach(p => p.IsDeleted = true);
            productAdditionOptionItemRepository.Update(items);
        }

        public List<AdditionalOption> GetAdditionalOptionsByBranchId(int branchId)
        {
            var optionItemsDict = additionOptionItemRepository
                .Get(p => p.BranchId == branchId)
                .GroupBy(p => p.AdditionOptionId)
                .ToDictionary(p => p.Key, p => p.ToList());
            var additionalOptions = additionalOptionRepository.Get(p => p.BranchId == branchId).ToList();

            additionalOptions.ForEach(p => 
            {
                optionItemsDict.TryGetValue(p.Id, out List<AdditionOptionItem> items);
                p.Items = items;
            });

            return additionalOptions;
        }

        public void RemoveProductByCategory(int categoryId)
        {
            var product = productRepository.Get(p => p.CategoryId == categoryId && !p.IsDeleted).ToList();
            product.ForEach(p => 
            {
                p.IsDeleted = true;
                SaveProductAdditionalFilling(p);
                SaveProductAdditionalOptions(p);
            });

            productRepository.Update(product);
        }

        public List<ProductModel> GetByCategory(int categoryId)
        {
            var products = productRepository.Get(p => p.CategoryId == categoryId && !p.IsDeleted).ToList();
            AppendProductAdditionalItems(products);

            return products;
        }
        
        public void RemoveProduct(int id)
        {
            var product = productRepository.Get(id);

            product.IsDeleted = true;
            productRepository.Update(product);

            RecalcProductsOrderNumber(product.CategoryId);
            SaveProductAdditionalFilling(product);
            SaveProductAdditionalOptions(product);
        }

        private void AppendProductAdditionalItems(List<ProductModel> products)
        {
            var productIds = products.Select(p => p.Id).ToList();

            var additionalFillings = GetProductAdditionalFillingsByProductIds(productIds);
            var additionalOptions = GetProductAdditionOptionItemByProductIds(productIds);

            products.ForEach(p =>
            {
                p.ProductAdditionalFillingIds = additionalFillings[p.Id].Select(x => x.AdditionalFillingId).ToList();
                p.ProductAdditionalOptionIds = additionalOptions[p.Id].Select(x => x.AdditionalOptionId).ToList();
            });
        }


        private void RecalcProductsOrderNumber(int categoryId)
        {
            var products = productRepository.Get(p => p.CategoryId == categoryId && !p.IsDeleted).ToList();
            
            for (var i = 0; i < products.Count; ++i)
            {
                products[i].OrderNumber = i + 1;
            }

            productRepository.Update(products);
        }

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            displayItemSettingLogic.UpdateOrder(productRepository, items);
        }

        public void UpdateVisible(UpdaterVisible update)
        {
            displayItemSettingLogic.UpdateVisible(productRepository, update);
        }
    }
}