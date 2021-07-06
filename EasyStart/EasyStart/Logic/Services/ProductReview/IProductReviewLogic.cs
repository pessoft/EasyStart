using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.ProductReview
{
    public interface IProductReviewLogic
    {
        void UpdateVisible(UpdaterVisible update);

        List<Models.ProductReview> Get(int productId);
    }
}
