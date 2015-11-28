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

        public int CatagoryID { get; set; }

        [Display(Name ="Product Description")]
        public string ShortDesc { get; set; }

        [Display(Name ="Details")]
        public string LongDesc { get; set; }

        [Display(Name ="Product Picture")]
        public string Image { get; set; }

        public decimal Price { get; set; }

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