using Shopping.Business;
using Shopping.Models;
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
            iBusinessAuth = new BusinessAuth();
            base.Initialize(requestContext);
        }

        // GET: Auth
        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string retUrl)
        {
            if (ModelState.IsValid)
            {
                if (iBusinessAuth.SignIn(model.Username, model.Password, false))
                {
                    if (!String.IsNullOrEmpty(retUrl))
                        return Redirect(retUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }
    }
}