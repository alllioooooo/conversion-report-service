using System.Reflection;
using ConversionReportService.Client.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient<ExceptionFormattingMiddleware>();
        services.AddTransient<RateLimitingMiddleware>();

        return services;
    }
}