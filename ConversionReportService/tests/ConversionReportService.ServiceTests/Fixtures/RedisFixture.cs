using ConversionReportService.Infrastructure.Caching.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;

namespace ConversionReportService.ServiceTests.Fixtures;

public class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer;
    private ConnectionMultiplexer _redis = null!;

    public RedisFixture()
    {
        _redisContainer = new RedisBuilder()
            .WithImage("redis:latest")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        _redis = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
        _redis.Dispose();
    }

    public IDatabase Database => _redis.GetDatabase();
    public string ConnectionString => _redisContainer.GetConnectionString();
    public IOptions<RedisOptions> GetOptions() =>
        Options.Create(new RedisOptions { ConnectionString = ConnectionString });
}