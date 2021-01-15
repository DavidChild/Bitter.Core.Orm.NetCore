using BT.Manage.Frame.Base.NetCore;
using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base
{
    public class CacheManager
    {
        private const string myCache = "myCacheBlock";
        private const string myhandleBlock = "myhandleBlock";
        Action<ConfigurationBuilderCachePart> sttins { get; set; }
        ICacheManager manager { get; set; }


        public  CacheManager(Action<ConfigurationBuilderCachePart> stetting)
        {
            sttins = stetting;
            ICacheManager manager =(ICacheManager) CacheFactory.Build(myhandleBlock,sttins);
        }
     

        
        public T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
             
        }

        public bool IsSet(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            
        }

        public void RemoveByPattern(string pattern)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object data, int cacheTime)
        {
            if (data != null)
            {
                manager.Set(AddKey(key), data, GetMemoryCacheEntryOptions(TimeSpan.FromMinutes(cacheTime)));
            }
        }

        /// <summary>
        /// Add key to dictionary
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>Itself key</returns>
        protected string AddKey(string key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }
    }
}
