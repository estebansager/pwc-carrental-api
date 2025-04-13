using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getData, TimeSpan? absoluteExpiration = null)
    {
        if (_memoryCache.TryGetValue(key, out T value))
            return value;

        value = await getData();

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
        };

        _memoryCache.Set(key, value, options);
        return value;
    }

    public void Set<T>(string key, T data, TimeSpan? absoluteExpiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
        };

        _memoryCache.Set(key, data, options);
    }

    public bool TryGet<T>(string key, out T value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }
}