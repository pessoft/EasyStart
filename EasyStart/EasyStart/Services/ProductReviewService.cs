using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.ProductReview;
using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class ProductReviewService
    {
        private readonly IProductReviewLogic productReviewLogic;

        public ProductReviewService(IProductReviewLogic productReviewLogic)
        {
            this.productReviewLogic = productReviewLogic;
        }
        
        public void UpdateVisible(UpdaterVisible update)
        {
            productReviewLogic.UpdateVisible(update);
        }
    }
}