using System.Text.Json;
using StackExchange.Redis;

namespace Cache.Data;

public class RedEfCachedCommand<TStore>: CachingCommand
{
    public RedEfCachedCommand(Func<Task> command, TStore item, IDatabase cache, Func<Task<string>> key) 
        : base(command, UseRedis(cache, key, item), () => true)
    {
        
    }

    private static Func<Task> UseRedis(IDatabase cache, Func<Task<string>> key, TStore item)
    {
        return async () =>
        {
            var content = JsonSerializer.Serialize(item);
            var wasSet = await cache.StringSetAsync(await key(), new RedisValue(content), TimeSpan.FromSeconds(100));
        };
    }
}