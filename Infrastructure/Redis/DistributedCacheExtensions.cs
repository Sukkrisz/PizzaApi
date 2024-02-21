using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Redis
{
    public static class DistributedCacheExtensions
    {
        // Used for storeing a value into redis
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordKey,
            T data,
            TimeSpan? expireTime = null,
            TimeSpan? unusedExipreTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            // How long the cache should be stored
            options.AbsoluteExpirationRelativeToNow = expireTime ?? TimeSpan.FromSeconds(60);

            // By default we use the absolute for expiration time
            // Sliding is used for checking for idle caches.
            // For example if absolute is 1hr, and the sliding is set to 5 mins, then if there's a lot of traffic
            // the cache will be stored for 1 hr, before it's refreshed.
            // But if no one accessed the data for 5 mins, then we'd get rid of the cache
            options.SlidingExpiration = unusedExipreTime;

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordKey, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordKey)
        {
            var jsonData = await cache.GetStringAsync(recordKey);

            if (jsonData is null)
            {
                return default(T)!;
            }

            return JsonSerializer.Deserialize<T>(jsonData)!;
        }
    }
}
