using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.ProductReview
{
    public class ProductReviewLogic: IProductReviewLogic
    {
        private readonly IBaseRepository<Models.ProductReview, int> productReviewRepository;
        private readonly IDisplayItemSettingLogic displayItemSettingLogic;

        public ProductReviewLogic(
            IBaseRepository<Models.ProductReview, int> productReviewRepository,
            IDisplayItemSettingLogic displayItemSettingLogic)
        {
            this.productReviewRepository = productReviewRepository;
            this.displayItemSettingLogic = displayItemSettingLogic;
        }

        public void UpdateVisible(UpdaterVisible update)
        {
            displayItemSettingLogic.UpdateVisible(productReviewRepository, update);
        }
    }
}