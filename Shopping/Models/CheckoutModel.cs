using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Models
{
    public class CheckoutModel
    {
        public CartListModel Cart { get; set; }

        public RegistrationModel Customer { get; set; }
    }
}