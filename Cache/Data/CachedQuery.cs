namespace Cache.Data;

public class CachedQuery<TReturn>
{
    private readonly Func<Task<TReturn>> _query;
    private readonly Func<Task<TReturn?>> _cache;

    protected CachedQuery(Func<Task<TReturn>> query, Func<Task<TReturn?>> cache)
    {
        _query = query;
        _cache = cache;
    }

    public async Task<TReturn> ExecuteAsync()
    {
        return await _cache() ?? await _query();
    }
}