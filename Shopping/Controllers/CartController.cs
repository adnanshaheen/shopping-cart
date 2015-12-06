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
                CartModel c1 = new CartModel();
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
            if (Request.Form["btnClear"] != null)
            {

            }
            else if (Request.Form["btnUpdate"] != null)
            {

            }
            else if (Request.Form["btnCancel"] != null)
            {

            }
            return View();
        }
    }
}