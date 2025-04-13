public interface ICacheService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getData, TimeSpan? absoluteExpiration = null);
    void Set<T>(string key, T data, TimeSpan? absoluteExpiration = null);
    bool TryGet<T>(string key, out T value);
}