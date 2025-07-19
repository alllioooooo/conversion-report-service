using System.Text.Json;
using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Infrastructure.Caching.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ConversionReportService.Infrastructure.Caching.RedisRepositories;

public sealed class CachedReportRepository : ICachedReportRepository
{
    private const string KeyPrefix = "conversion_report";
    private static ConnectionMultiplexer? _connection;
    private readonly string _redisConnectionString;

    public CachedReportRepository(IOptions<RedisOptions> options)
    {
        _redisConnectionString = options.Value.ConnectionString;
    }

    private async Task<IDatabase> GetDb()
    {
        _connection ??= await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);
        return _connection.GetDatabase();
    }

    private static string GetKey(long registrationId) => $"{KeyPrefix}:{registrationId}";

    public async Task SetAsync(ConversionReportDbo dbo, TimeSpan ttl, CancellationToken cancellationToken)
    {
        var db = await GetDb();
        var key = GetKey(dbo.RegistrationId);
        var value = JsonSerializer.Serialize(dbo);
        await db.StringSetAsync(key, value, ttl);
    }

    public async Task<ConversionReportDbo?> GetAsync(long registrationId, CancellationToken cancellationToken)
    {
        var db = await GetDb();
        var value = await db.StringGetAsync(GetKey(registrationId));

        return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ConversionReportDbo>(value);
    }
}