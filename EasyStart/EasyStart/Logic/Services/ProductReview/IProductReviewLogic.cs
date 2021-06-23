using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.ProductReview
{
    public interface IProductReviewLogic
    {
        void UpdateVisible(UpdaterVisible update);

        List<Models.ProductReview> Get(int productId);
    }
}
