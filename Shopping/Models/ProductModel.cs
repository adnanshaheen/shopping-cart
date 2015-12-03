using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ProductModel : IEntity
    {
        public int ProductID { get; set; }

        [Display(Name ="Catagory")]
        public int CatagoryID { get; set; }

        [Display(Name ="Product Description")]
        [Required(ErrorMessage ="Short description is required")]
        [StringLength(50, ErrorMessage = "Length can't exceed 50 charectars")]
        public string ShortDesc { get; set; }

        [Display(Name ="Details")]
        [Required(ErrorMessage ="Long description is required")]
        public string LongDesc { get; set; }

        [Display(Name ="Product Picture")]
        public string Image { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

        [Required(ErrorMessage ="Price is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid value")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Inventory is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid value")]
        public int Inventory { get; set; }

        #region IEntity Members
        public void SetFeilds(DataRow dataRow)
        {
            ProductID = (int)dataRow["ProductId"];
            CatagoryID = (int)dataRow["CatID"];
            ShortDesc = (string)dataRow["ProductSDesc"];
            LongDesc = (string)dataRow["ProductLDesc"];
            Image = (string)dataRow["ProductImage"];
            Price = (decimal)dataRow["Price"];
            Inventory = (int)dataRow["Inventory"];
        }
        #endregion
    }
}