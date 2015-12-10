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
            CartModel cart = new CartModel();
            ProductModel product = null;
            try
            {
                if (!string.IsNullOrEmpty(prodID))
                    product = iBusinessShop.GetProduct(prodID);

                ProductCart.Product = product;
                cart.ProductQuantity = 1;
                ProductCart.Cart = cart;
            }
            catch (Exception)
            {
                throw;
            }
            return View(ProductCart);
        }

        [HttpPost]
        public ActionResult Details(ProductCartModel model)
        {
            try
            {
                if (Request.Form["btnContShopping"] != null)
                {
                    return RedirectToAction("Index", "Products", new { catID = model.Product.CatagoryID });
                }
                else if (Request.Form["btnViewCart"] != null)
                {
                    return RedirectToAction("ViewCart", "Cart");
                }
                else if (Request.Form["btnAddToCart"] != null)
                {
                    // We can retrieve the product information from database
                    List<CartModel> cartCookieList = new List<CartModel>();
                    CookieHelper<List<CartModel>>.GetValueFromCookie("cart", ref cartCookieList);

                    var item = cartCookieList.Find(cart => cart.ProductID == model.Product.ProductID);
                    int ProductQuantity = 0;
                    if (item != null)
                    {
                        item.ProductQuantity += model.Cart.ProductQuantity;
                        ProductQuantity = item.ProductQuantity;
                    }
                    else
                    {
                        model.Cart.ProductID = model.Product.ProductID;
                        cartCookieList.Add(model.Cart);
                        ProductQuantity = model.Cart.ProductQuantity;
                    }

                    model.Product = iBusinessShop.GetProduct(model.Product.ProductID.ToString());

                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", cartCookieList, DateTime.MaxValue);
                    model.Product.Status = ProductQuantity.ToString() + " items of " +
                        model.Product.ShortDesc + " added to your cart.";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        public ActionResult Product(int? ProdID)
        {
            ProductModel model = null;
            try
            {
                if (ProdID != null)
                {
                    model = iBusinessShop.GetProduct(ProdID.ToString());
                    model.Update = true;
                }
                else
                {
                    model = new ProductModel();
                    model.Update = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Product(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                bool bRes = false;
                if (model.Update)
                    bRes = iBusinessShop.UpdateProduct(model);
                else
                    bRes = iBusinessShop.AddProduct(model);

                if (bRes)
                    model.Status = "Product " + (model.Update ? "updated" : "added") + " successfully";
                else
                    model.Status = "Couldn't " + (model.Update ? "update" : "add") + " product";
            }
            return View(model);
        }

        public ActionResult ViewProducts()
        {
            List<ProductModel> TList = null;
            try
            {
                if (iBusinessShop != null)
                {
                    TList = iBusinessShop.GetProducts("");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(TList);
        }
    }
}