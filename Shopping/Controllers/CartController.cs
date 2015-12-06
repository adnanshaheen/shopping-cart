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
    public class CartController : Controller
    {
        private IBusinessShop iBusinessShop = null;

        protected override void Initialize(RequestContext requestContext)
        {
            iBusinessShop = GenericFactory<BusinessShop, IBusinessShop>.CreateInstance();
            base.Initialize(requestContext);
        }

        // GET: Cart
        public ActionResult ViewCart()
        {
            List<CartModel> cartCookieList = null;
            try
            {
                cartCookieList = new List<CartModel>();
                CookieHelper<List<CartModel>>.GetValueFromCookie("cart", ref cartCookieList);
                foreach (var item in cartCookieList)
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

            return View(cartCookieList);
        }

        [HttpPost]
        public ActionResult ViewCart(List<CartModel> model)
        {
            // FIXME: model is empty
            // either search for a way to return the whole list back from view
            // or make another model class and have List<CartMode> as it's object
            if (Request.Form["btnClear"] != null)
            {
                if (model.Any())
                {
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", model, DateTime.Now.AddDays(-1d));
                    CookieHelper<List<CartModel>>.SetValueToCookie("oldcart", model, DateTime.MaxValue);
                    model.Clear();
                }
            }
            else if (Request.Form["btnUpdate"] != null)
            {
                if (model.Any())
                {
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", model, DateTime.MaxValue);
                    CookieHelper<List<CartModel>>.SetValueToCookie("oldcart", model, DateTime.MaxValue);
                }
            }
            else if (Request.Form["btnCancel"] != null)
            {
                CookieHelper<List<CartModel>>.GetValueFromCookie("oldcart", ref model);
                if (model.Any())
                {
                    CookieHelper<List<CartModel>>.SetValueToCookie("oldcart", model, DateTime.Now.AddDays(-1d));
                    CookieHelper<List<CartModel>>.SetValueToCookie("cart", model, DateTime.MaxValue);
                    foreach (var item in model)
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
            return View(model);
        }
    }
}