using Shopping.Models;
using System.Collections.Generic;

namespace Shopping.Business
{
    internal interface IBusinessShop
    {
        List<ProductModel> GetProducts(string catID);
    }
}