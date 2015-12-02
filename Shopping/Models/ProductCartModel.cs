using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Models
{
    public class ProductCartModel
    {
        public ProductModel Product { get; set; }
        public CartModel Cart { get; set; }
    }
}