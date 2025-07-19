using ConversionReportService.Application.Contracts.Services;
using ConversionReportService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IItemInteractionService, ItemInteractionService>();
        services.AddScoped<IConversionReportService, Services.ConversionReportService>();

        return services;
    }
}