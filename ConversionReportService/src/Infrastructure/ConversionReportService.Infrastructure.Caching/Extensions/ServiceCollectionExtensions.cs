using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Infrastructure.Caching.Options;
using ConversionReportService.Infrastructure.Caching.RedisRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Infrastructure.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RedisOptions>().BindConfiguration("Caching:Redis");

        services.AddSingleton<ICachedReportRepository, CachedReportRepository>();

        return services;
    }
}