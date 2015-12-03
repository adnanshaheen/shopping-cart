using Shopping.Business;
using Shopping.Models;
using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shopping.Controllers
{
    public class ProductsController : Controller
    {
        private IBusinessShop iBusinessShop = null;

        protected override void Initialize(RequestContext requestContext)
        {
            iBusinessShop = GenericFactory<BusinessShop, IBusinessShop>.CreateInstance();
            base.Initialize(requestContext);
        }

        // GET: Products
        public ActionResult Index(string catID)
        {
            List<ProductModel> TList = null;
            try
            {
                if (iBusinessShop != null)
                {
                    if (string.IsNullOrEmpty(catID))
                        catID = "10";
                    TList = iBusinessShop.GetProducts(catID);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(TList);
        }

        public ActionResult Details(string prodID)
        {
            ProductCartModel ProductCart = new ProductCartModel();
            ProductModel product = null;
            try
            {
                if (!string.IsNullOrEmpty(prodID))
                    product = iBusinessShop.GetProduct(prodID);

                ProductCart.Product = product;
            }
            catch (Exception)
            {
                throw;
            }
            return View(ProductCart);
        }

        public ActionResult AddProduct()
        {
            ProductModel model = new ProductModel();
            return View(model);
        }
    }
}