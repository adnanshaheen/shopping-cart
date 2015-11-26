using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Shopping
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (Request.IsAuthenticated) // find all roles for this user
            {
                // Extract the forms authentication cookie
                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = Context.Request.Cookies[cookieName];
                if (null == authCookie)
                {
                    return;  // no authentication cookie.
                }
                FormsAuthenticationTicket authTicket = null;
                try
                {
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch (Exception)
                {
                    return;  // tod, log exception details 
                }
                if (null == authTicket)
                    return;  // Cookie failed to decrypt.

                // When the ticket was created, the UserData property was assigned
                // a pipe delimited string of role names.
                string roles = authTicket.UserData;
                string[] roleListArray = roles.Split(new char[] { '|' });
                // Add the roles obtained above to the User Principal
                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(User.Identity, roleListArray);
            }
        }
    }
}
