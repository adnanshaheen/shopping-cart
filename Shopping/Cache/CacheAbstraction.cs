using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Cache
{
    public class CacheAbstraction
    {
        private IWebCache iWebCache = null;

        public CacheAbstraction(IWebCache iCache)
        {
            iWebCache = iCache;
        }

        public CacheAbstraction() :
            this(GenericFactory<HttpContextCacheAdapter, IWebCache>.CreateInstance())
        {
        }

        public void Remove(string key)
        {
            iWebCache.Remove(key);
        }

        public void Insert(string key, object obj)
        {
            iWebCache.Store(key, obj);
        }

        public T Retrieve<T>(string key)
        {
            return iWebCache.Retrieve<T>(key);
        }
    }
}