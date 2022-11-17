namespace Cache.Data;

public class CachingCommand
{
    private readonly Func<Task> _command;
    private readonly Func<Task> _cache;
    private readonly Func<bool> _cacheCondition;

    protected CachingCommand(Func<Task> command, Func<Task> cache, Func<bool> cacheCondition)
    {
        _command = command;
        _cache = cache;
        _cacheCondition = cacheCondition;
    }

    public async Task<bool> ExecuteAsync()
    {
        await _command();
        var isCached = _cacheCondition();
        
        if (isCached)
            await _cache();
        
        return isCached;
    }
}