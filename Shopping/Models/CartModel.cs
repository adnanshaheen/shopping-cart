using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopping.Models
{
    public class CartModel
    {
        [Display(Name ="Product ID")]
        public int ProductID { get; set; }

        [Display(Name ="Product Name")]
        public string ProductName { get; set; }

        [Display(Name ="Price")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage ="Please enter a quantity")]
        [Range(1, int.MaxValue, ErrorMessage ="Please enter a valid value")]    // range isn't working
        public int ProductQuantity { get; set; }
    }
}