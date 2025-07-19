using ConversionReportService.Infrastructure.BackgroundServices.MigrationsService;
using ConversionReportService.Infrastructure.BackgroundServices.ReportProcessingService;
using ConversionReportService.Infrastructure.BackgroundServices.ReportStatusCheckupService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Infrastructure.BackgroundServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundReportStatusCheckupService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ReportStatusCheckupOptions>().BindConfiguration("BackgroundServices:ReportStatusCheckup");

        services.AddHostedService<BackgroundReportStatusCheckupService>();

        return services;
    }

    public static IServiceCollection AddBackgroundMigrationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<BackgroundMigrationsService>();
        return services;
    }

    public static IServiceCollection AddBackgroundReportProcessingService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ReportProcessingServiceOptions>().BindConfiguration("BackgroundServices:ReportProcessing");

        services.AddHostedService<BackgroundReportProcessingService>();

        return services;
    }
}