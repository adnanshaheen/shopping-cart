using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;

namespace Shopping.Models
{
    public class ImageModel : IEntity
    {
        [Display(Name = "Product Picture")]
        public HttpPostedFileBase ImageFile { get; set; }

        public Byte[] ImageData { get; set; }

        public string Type { get; set; }

        public void SetFeilds(DataRow dataRow)
        {
            ImageData = (Byte[])dataRow["Image"];
            Type = (string)dataRow["Type"];
        }
    }
}