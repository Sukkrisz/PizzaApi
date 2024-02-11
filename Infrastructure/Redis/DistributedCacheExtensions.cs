using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Redis
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordKey,
            T data,
            TimeSpan? expireTime = null,
            TimeSpan? unusedExipreTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = expireTime ?? TimeSpan.FromSeconds(60);
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
