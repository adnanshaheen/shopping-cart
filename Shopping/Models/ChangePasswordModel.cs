using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopping.Models
{
    public class ChangePasswordModel
    {
        [StringLength(128, ErrorMessage = "The {0} must be atleast {2} characters long", MinimumLength = 8)]
        [Required(ErrorMessage = "Password is required ...")]
        [Display(Name = "Password")]
        public string oldPassword { get; set; }

        [StringLength(128, ErrorMessage = "The {0} must be atleast {2} characters long", MinimumLength = 8)]
        [Required(ErrorMessage = "New password is required ...")]
        [Display(Name = "New Password")]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "New password is required ...")]
        [Display(Name = "Re-type New")]
        [CompareAttribute("newPassword", ErrorMessage = "Passwords don't match.")]
        public string reNewPassword { get; set; }

        public string Status { get; set; }
    }
}