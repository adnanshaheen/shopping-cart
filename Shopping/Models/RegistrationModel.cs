using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;

namespace Shopping.Models
{
    public class RegistrationModel : IEntity
    {
        [Required(ErrorMessage = "First name is required ...")]
        [StringLength(50, ErrorMessage = "Length can't exceed 50 charectars")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required ...")]
        [StringLength(50, ErrorMessage = "Length can't exceed 50 charectars")]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required..")]
        [StringLength(100, ErrorMessage = "Length cannot exceed 100 chars")]
        [Display(Name ="Address")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required..")]
        [StringLength(50, ErrorMessage = "Length cannot exceed 50 chars")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required..")]
        [StringLength(2, ErrorMessage = "only 2 char abbrev. for state is needed")]
        public string State { get; set; }

        [Required(ErrorMessage = "zip is required..")]
        [RegularExpression("^([0-9]{5})$", ErrorMessage = "invalid zip code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Email is required ...")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Display(Name ="Credit Card Number")]
        public string CreditCard { get; set; }

        [Display(Name ="Credit Card Type")]
        public int CreditCardType { get; set; }

        [Display(Name ="Credit Card Expiration")]
        public string Expiration { get; set; }

        [Display(Name ="Username")]
        [Required(ErrorMessage = "Username is required..")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required..")]
        [StringLength(128, ErrorMessage = "The {0} must be atleast {2} characters long", MinimumLength = 8)]
        public string Password { get; set; }

        [Display(Name ="Confirm Password")]
        [CompareAttribute("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPwd { get; set; }

        [Display(Name ="Password Hint Question")]
        public string PasswordHintQ { get; set; }

        [Display(Name ="Password Hint Answer")]
        public string PasswordHintA { get; set; }

        public string Status { get; set; }

        public void SetFeilds(DataRow dataRow)
        {
            FirstName = (string)dataRow["FirstName"];
            LastName = (string)dataRow["LastName"];
            StreetAddress = (string)dataRow["Address"];
            ZipCode = (string)dataRow["Zipcode"];
            City = (string)dataRow["City"];
            State = (string)dataRow["State"];
            CreditCard = (string)dataRow["CCNumber"];
            Expiration = (string)dataRow["CCExpiration"];
            CreditCardType = (int)dataRow["CCType"];
            Email = (string)dataRow["Email"];
        }
    }
}