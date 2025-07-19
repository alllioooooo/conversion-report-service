using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Presentation.Http.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpPresentation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}