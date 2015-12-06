﻿using Shopping.Business;
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
                    if (item != null)
                    {
                        ++item.ProductQuantity;
                    }
                    else
                    {
                        model.Cart.ProductID = model.Product.ProductID;
                        cartCookieList.Add(model.Cart);
                    }
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", cartCookieList, DateTime.MaxValue);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        public ActionResult AddProduct()
        {
            ProductModel model = new ProductModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddProduct(ProductModel model)
        {
            if (ModelState.IsValid)
                iBusinessShop.AddProduct(model);
            return View(model);
        }
    }
}