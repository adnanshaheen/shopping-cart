using Shopping.Business;
using Shopping.Models;
using Shopping.Utilities;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shopping.Controllers
{
    [HandleError]
    public class AuthController : Controller
    {
        private IBusinessAuth iBusinessAuth = null;

        protected override void Initialize(RequestContext requestContext)
        {
            iBusinessAuth = GenericFactory<BusinessShop, IBusinessAuth>.CreateInstance();
            base.Initialize(requestContext);
        }

        // GET: Auth
        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                if (iBusinessAuth.SignIn(model.Username, model.Password, false))
                {
                    if (!String.IsNullOrEmpty(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            iBusinessAuth.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (iBusinessAuth.ChangePassword(HttpContext.User.Identity.Name, model.oldPassword, model.newPassword))
                    model.Status = "Password updated succesfully.";
                else
                    model.Status = "Couldn't update the password.";
            }
            return View(model);
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationModel model)
        {
            return View(model);
        }
    }
}