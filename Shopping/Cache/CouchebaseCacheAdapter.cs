using Couchbase;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Cache
{
    public class CouchebaseCacheAdapter : IWebCache
    {
        private CouchbaseClient client = null;

        public CouchebaseCacheAdapter()
        {
            client = new CouchbaseClient();
        }

        #region IWebCache Members
        public void Remove(string key)
        {
            client.Remove(key);
        }

        public T Retrieve<T>(string key)
        {
            return client.Get<T>(key);
        }

        public void Store(string key, object obj)
        {
            client.Store(StoreMode.Set, key, obj, TimeSpan.FromMinutes(1));
        }
        #endregion
    }
}