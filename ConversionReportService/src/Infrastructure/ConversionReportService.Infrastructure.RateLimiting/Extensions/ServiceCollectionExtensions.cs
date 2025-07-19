using ConversionReportService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Infrastructure.RateLimiting.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRateLimiter, RedisRateLimiter>();
        return services;
    }
}