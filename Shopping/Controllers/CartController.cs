using Shopping.Models;
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
            List<CartModel> Cart = new List<CartModel>();
            //CartModel c1 = new CartModel { ProductID = 1, ProductName = "Casio HandHeld Color1 TV 2.7", ProductPrice = 45, ProductQuantity = 1 };
            //CartModel c2 = new CartModel { ProductID = 2, ProductName = "Shamshatoo2", ProductPrice = 45, ProductQuantity = 1 };
            //CartModel c3 = new CartModel { ProductID = 3, ProductName = "Shamshatoo3", ProductPrice = 45, ProductQuantity = 2 };
            //CartModel c4 = new CartModel { ProductID = 4, ProductName = "Shamshatoo4", ProductPrice = 45, ProductQuantity = 3 };
            //CartModel c5 = new CartModel { ProductID = 5, ProductName = "Shamshatoo5", ProductPrice = 45, ProductQuantity = 1 };
            //Cart.Add(c1);
            //Cart.Add(c2);
            //Cart.Add(c3);
            //Cart.Add(c4);
            //Cart.Add(c5);
            return View(Cart);
        }
    }
}