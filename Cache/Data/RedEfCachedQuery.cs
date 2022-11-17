using System.Text.Json;
using StackExchange.Redis;

namespace Cache.Data;

public class RedEfCachedQuery<TReturn>: CachedQuery<TReturn>
{
    public RedEfCachedQuery(Func<Task<TReturn>> query, IDatabase cache, string key) : base(query, UseRedis(cache, key))
    {
        
    }

    private static Func<Task<TReturn?>> UseRedis(IDatabase cache, string key)
    {
        return async () =>
        {
            var content = await cache.StringGetAsync(key);
            if (!content.HasValue)
                return default;
            var obj = JsonSerializer.Deserialize<TReturn>(content);
            return obj;
        };
    }
}