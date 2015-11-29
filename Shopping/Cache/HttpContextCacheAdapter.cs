using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Cache
{
    public class HttpContextCacheAdapter : IWebCache
    {
        #region IWebCache Members
        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public T Retrieve<T>(string key)
        {
            T item = (T)HttpContext.Current.Cache.Get(key);
            if (item == null)
                item = default(T);
            return item;
        }

        public void Store(string key, object obj)
        {
            HttpContext.Current.Cache.Insert(key, obj);
        }
        #endregion
    }
}