using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Shopping.Utilities
{
    public class CookieHelper<T>
    {
        CookieHelper()
        {

        }

        public static bool GetValueFromCookie(string cookieName, ref T value)
        {
            HttpCookie cookie = null;
            if (HttpContext.Current.Request.Cookies.AllKeys.Any(key => key.Equals(cookieName)))
                cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie == null || String.IsNullOrEmpty(cookie.Value))
                return false;

            var javaScriptSerializer = new JavaScriptSerializer();
            value = javaScriptSerializer.Deserialize<T>(cookie.Value);
            return true;
        }

        public static void SetValueToCookie(string name, T value, DateTime expires)
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            var cookie = new HttpCookie(name) { Value = javaScriptSerializer.Serialize(value), Expires = expires };
            HttpContext.Current.Response.SetCookie(cookie);
        }
    }
}