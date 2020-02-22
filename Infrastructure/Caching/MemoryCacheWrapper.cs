using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Infrastructure.Caching
{
    public class MemoryCacheWrapper
    {
        private static readonly Dictionary<string, object> LOCKS = new Dictionary<string, object>();

        public static T AddOrGetExisting<T>(string publicKey, ExpirationType expirationType, TimeSpan expirationInterval, Func<T> loader, Action<CacheEntryUpdateArguments> expirationCallback = null)
            where T : class
        {
            var privateKey = KeyTransform<T>(publicKey);

            // Returns null if the string does not exist, prevents a race condition where the cache invalidates between the contains check and the retreival.
            var cachedItem = MemoryCache.Default.Get(privateKey) as CacheItem;

            if (cachedItem != null)
            {
                return cachedItem.Value as T;
            }

            lock (LOCKS[privateKey])
            {
                // Check to see if anyone wrote to the cache while we were waiting our turn to write the new value.
                cachedItem = MemoryCache.Default.Get(privateKey) as CacheItem;

                if (cachedItem != null)
                {
                    return cachedItem.Value as T;
                }

                var policy = new CacheItemPolicy();

                Action<CacheEntryUpdateArguments> defaultExpirationCallback = DefaultExpiredBehavior;
                var callback = expirationCallback != null ? (Action<CacheEntryUpdateArguments>)Delegate.Combine(defaultExpirationCallback, expirationCallback) : defaultExpirationCallback;
                policy.UpdateCallback = e => callback(e);

                switch (expirationType)
                {
                    case ExpirationType.Absolute:
                        policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now + expirationInterval);
                        break;
                    case ExpirationType.Sliding:
                        policy.SlidingExpiration = expirationInterval;
                        break;
                }

                // The value still did not exist so we now write it in to the cache.
                var data = loader();

                var cacheItem = new CacheItem(privateKey, data);
                MemoryCache.Default.Set(privateKey, cacheItem, policy);

                return data;
            }
        }

        public static T Get<T>(string publicKey) where T : class
        {
            var privateKey = KeyTransform<T>(publicKey);

            var cachedItem = MemoryCache.Default.Get(privateKey) as CacheItem;

            if (cachedItem != null)
            {
                return cachedItem.Value as T;
            }

            return null;
        }

        public static void Remove<T>(string publicKey)
        {
            var privateKey = KeyTransform<T>(publicKey);

            MemoryCache.Default.Remove(privateKey);
        }

        private static string KeyTransform<T>(string publicKey)
        {
            var privateKey = string.Format("{0}-{1}", publicKey, typeof(T));

            if (!LOCKS.ContainsKey(privateKey))
            {
                LOCKS.Add(privateKey, new object());
            }

            return privateKey;
        }

        public static void DefaultExpiredBehavior(CacheEntryUpdateArguments args)
        {
            const char SEPARATOR = '-';

            var split = args.Key.Split(SEPARATOR);

            var key = split.First();
            var type = split.Last();
        }
    }
}
