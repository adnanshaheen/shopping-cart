using Shopping.Models;
using System.Collections.Generic;

namespace Shopping.Data
{
    interface IRepositoryShop
    {
        List<ProductModel> GetProducts(string catID);
    }
}
