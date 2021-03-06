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
    public class CartController : Controller
    {
        private IBusinessShop iBusinessShop = null;
        private IBusinessAuth iBusinessAuth = null;

        protected override void Initialize(RequestContext requestContext)
        {
            iBusinessShop = GenericFactory<BusinessShop, IBusinessShop>.CreateInstance();
            iBusinessAuth = GenericFactory<BusinessShop, IBusinessAuth>.CreateInstance();
            base.Initialize(requestContext);
        }

        // GET: Cart
        public ActionResult ViewCart()
        {
            CartListModel cartList = new CartListModel();
            List<CartModel> cartCookieList = null;
            try
            {
                cartCookieList = new List<CartModel>();
                CookieHelper<List<CartModel>>.GetValueFromCookie("cart", ref cartCookieList);
                cartList.CartList = cartCookieList;
                foreach (var item in cartList.CartList)
                {
                    ProductModel product = iBusinessShop.GetProduct(item.ProductID.ToString());
                    if (product != null)
                    {
                        item.ProductName = product.ShortDesc;
                        item.ProductPrice = product.Price;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(cartList);
        }

        [HttpPost]
        public ActionResult ViewCart(CartListModel model)
        {
            model = Cart(model);
            return View(model);
        }

        [Authorize]
        public ActionResult Checkout()
        {
            CheckoutModel model = new CheckoutModel();
            model.Cart = new CartListModel();
            List<CartModel> cartCookieList = null;
            try
            {
                cartCookieList = new List<CartModel>();
                CookieHelper<List<CartModel>>.GetValueFromCookie("cart", ref cartCookieList);
                model.Cart.CartList = cartCookieList;
                foreach (var item in model.Cart.CartList)
                {
                    ProductModel product = iBusinessShop.GetProduct(item.ProductID.ToString());
                    if (product != null)
                    {
                        item.ProductName = product.ShortDesc;
                        item.ProductPrice = product.Price;
                    }
                }

                model.Customer = iBusinessAuth.GetCustomerInfo(HttpContext.User.Identity.Name);
            }
            catch (Exception)
            {
                throw;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutModel model)
        {
            try
            {
                if (Request.Form["btnUpdateCustomer"] != null)
                {
                    if (iBusinessAuth.UpdateCustomer(model.Customer))
                        model.Customer.Status = model.Customer.LastName + "'s information updated successfully";
                    else
                        model.Customer.Status = "Couldn't update customer information";
                }
                else if (Request.Form["btnCheckout"] != null)
                {
                    model.Customer.UserName = HttpContext.User.Identity.Name;
                    if (iBusinessShop.PlaceOrder(model))
                        model.Customer.Status = "Order placed successfully";
                    else
                        model.Customer.Status = "Couldn't place your order";

                    foreach (var item in model.Cart.CartList)
                    {
                        ProductModel product = iBusinessShop.GetProduct(item.ProductID.ToString());
                        if (product != null)
                        {
                            item.ProductName = product.ShortDesc;
                            item.ProductPrice = product.Price;
                        }
                    }
                }
                else if (Request.Form["btnClear"] != null ||
                    Request.Form["btnUpdate"] != null ||
                    Request.Form["btnCancel"] != null)
                {
                    model.Cart = Cart(model.Cart);
                }
            }
            catch (Exception ex)
            {
                model.Customer.Status = ex.Message;
            }
            return View(model);
        }

        private CartListModel Cart(CartListModel model)
        {
            List<CartModel> cartCookieList = null;
            if (Request.Form["btnClear"] != null)
            {
                if (model.CartList != null && model.CartList.Any())
                {
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", model.CartList, DateTime.Now.AddDays(-1d));
                    CookieHelper<List<CartModel>>.SetValueToCookie("oldcart", model.CartList, DateTime.MaxValue);
                    model.CartList.Clear();
                }
            }
            else if (Request.Form["btnUpdate"] != null)
            {
                if (model.CartList != null && model.CartList.Any())
                {
                    CookieHelper<List<CartModel>>.GetValueFromCookie("cart", ref cartCookieList);
                    CookieHelper<List<CartModel>>.SetValueToCookie("oldcart", cartCookieList, DateTime.MaxValue);
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", model.CartList, DateTime.MaxValue);
                    foreach (var item in model.CartList)
                    {
                        ProductModel product = iBusinessShop.GetProduct(item.ProductID.ToString());
                        if (product != null)
                        {
                            item.ProductName = product.ShortDesc;
                            item.ProductPrice = product.Price;
                        }
                    }
                }
            }
            else if (Request.Form["btnCancel"] != null)
            {
                if (model.CartList != null)
                {
                    CookieHelper<List<CartModel>>.GetValueFromCookie("oldcart", ref cartCookieList);
                    CookieHelper<List<CartModel>>.SetValueToCookie("oldcart", model.CartList, DateTime.Now.AddDays(-1d));
                    model.CartList = cartCookieList;
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", model.CartList, DateTime.MaxValue);
                    foreach (var item in model.CartList)
                    {
                        ProductModel product = iBusinessShop.GetProduct(item.ProductID.ToString());
                        if (product != null)
                        {
                            item.ProductName = product.ShortDesc;
                            item.ProductPrice = product.Price;
                        }
                    }
                }
            }
            if (model.CartList == null)
                model.CartList = new List<CartModel>();

            return model;
        }
    }
}