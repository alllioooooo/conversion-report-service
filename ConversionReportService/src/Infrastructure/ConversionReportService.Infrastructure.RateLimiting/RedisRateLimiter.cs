using ConversionReportService.Application.Abstractions;
using ConversionReportService.Infrastructure.Caching.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ConversionReportService.Infrastructure.RateLimiting;

public sealed class RedisRateLimiter : IRateLimiter
{
    private static ConnectionMultiplexer? _connection;
    private readonly string _connectionString;
    private readonly TimeSpan _window = TimeSpan.FromMinutes(1);
    private readonly int _maxRequests = 100;

    public RedisRateLimiter(IOptions<RedisOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    private async Task<IDatabase> GetDb()
    {
        _connection ??= await ConnectionMultiplexer.ConnectAsync(_connectionString);
        return _connection.GetDatabase();
    }

    public async Task<bool> AllowAsync(string key, CancellationToken cancellationToken = default)
    {
        var db = await GetDb();
        var redisKey = $"ratelimit:{key}";

        var count = await db.StringIncrementAsync(redisKey);

        if (count == 1)
        {
            await db.KeyExpireAsync(redisKey, _window);
        }

        return count <= _maxRequests;
    }
}