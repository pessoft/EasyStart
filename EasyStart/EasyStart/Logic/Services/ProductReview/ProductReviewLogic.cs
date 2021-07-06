using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Repository;
using System.Collections.Generic;
using System.Linq;

namespace EasyStart.Logic.Services.ProductReview
{
    public class ProductReviewLogic : IProductReviewLogic
    {
        private readonly IRepository<Models.ProductReview, int> productReviewRepository;
        private readonly IDisplayItemSettingLogic displayItemSettingLogic;

        public ProductReviewLogic(
            IRepositoryFactory repositoryFactory,
            IDisplayItemSettingLogic displayItemSettingLogic)
        {
            productReviewRepository = repositoryFactory.GetRepository<Models.ProductReview, int>();
            this.displayItemSettingLogic = displayItemSettingLogic;
        }

        public void UpdateVisible(UpdaterVisible update)
        {
            displayItemSettingLogic.UpdateVisible(productReviewRepository, update);
        }

        public List<Models.ProductReview> Get(int productId)
        {
            return productReviewRepository.Get(p => p.ProductId == productId)
                         .OrderByDescending(p => p.Date)
                         .Take(50)
                         .ToList();
        }
    }
}