using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Shopping.Models
{
    public class ProductModel : IEntity
    {
        public int CatagoryID { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int Inventory { get; set; }

        #region IEntity Members
        public void SetFeilds(DataRow dataRow)
        {
            CatagoryID = (int)dataRow["CatID"];
            ShortDesc = (string)dataRow["ProductSDesc"];
            LongDesc = (string)dataRow["ProductLDesc"];
            Image = (string)dataRow["ProductImage"];
            Price = (double)dataRow["Price"];
            Inventory = (int)dataRow["Inventory"];
        }
        #endregion
    }
}