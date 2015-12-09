using Shopping.Models;
using System.Collections.Generic;

namespace Shopping.Business
{
    internal interface IBusinessShop
    {
        List<ProductModel> GetProducts(string catID);

        ProductModel GetProduct(string prodId);

        bool AddProduct(ProductModel product);

        bool UpdateProduct(ProductModel product);

        bool PlaceOrder(CheckoutModel order);
    }
}