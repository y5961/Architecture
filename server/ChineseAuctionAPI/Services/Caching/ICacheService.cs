using System.Text.Json;
using StackExchange.Redis;

namespace ChineseAuctionAPI.Services.Caching
{
    /// <summary>
    /// Interface for caching service - abstracts Redis implementation
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
    }

    /// <summary>
    /// Redis cache implementation
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IConfiguration _configuration;

        public RedisCacheService(
            IConnectionMultiplexer redis,
            ILogger<RedisCacheService> logger,
            IConfiguration configuration)
        {
            _redis = redis;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Get value from cache
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var value = await db.StringGetAsync(key);

                if (!value.HasValue)
                {
                    _logger.LogInformation($"Cache miss for key: {key}");
                    return default;
                }

                _logger.LogInformation($"Cache hit for key: {key}");
                var json = value.ToString();
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting value from cache for key: {key}");
                return default;
            }
        }

        /// <summary>
        /// Set value in cache with optional TTL
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            try
            {
                var db = _redis.GetDatabase();
                var json = JsonSerializer.Serialize(value);
                
                // Use configured default TTL if not specified
                if (ttl == null)
                {
                    var defaultTtlSeconds = _configuration.GetValue<int>("Redis:DefaultTtlSeconds", 3600);
                    ttl = TimeSpan.FromSeconds(defaultTtlSeconds);
                }

                await db.StringSetAsync(key, json, ttl);
                _logger.LogInformation($"Cached value for key: {key} with TTL: {ttl.Value.TotalSeconds} seconds");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting value in cache for key: {key}");
            }
        }

        /// <summary>
        /// Remove value from cache
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(key);
                _logger.LogInformation($"Removed cache entry for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache entry for key: {key}");
            }
        }

        /// <summary>
        /// Remove multiple entries by pattern (useful for cache invalidation)
        /// </summary>
        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern).ToList();

                if (keys.Count == 0)
                {
                    _logger.LogInformation($"No keys found matching pattern: {pattern}");
                    return;
                }

                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(keys.ToArray());
                _logger.LogInformation($"Removed {keys.Count} cache entries matching pattern: {pattern}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache entries by pattern: {pattern}");
            }
        }

        /// <summary>
        /// Check if key exists in cache
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                return await db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking cache key existence: {key}");
                return false;
            }
        }
    }
}
