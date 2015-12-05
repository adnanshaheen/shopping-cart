using Shopping.Models;
using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult ViewCart()
        {
            List<CartModel> cartCookieList = null;
            try
            {
                cartCookieList = new List<CartModel>();
                CartModel c1 = new CartModel();
                CookieHelper<CartModel>.GetValueFromCookie("cart", ref c1);
                cartCookieList.Add(c1);
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