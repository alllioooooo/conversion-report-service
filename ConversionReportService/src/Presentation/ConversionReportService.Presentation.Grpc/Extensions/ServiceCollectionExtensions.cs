using ConversionReportService.Presentation.Grpc.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Presentation.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ReportGrpcExceptionInterceptor>();
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ReportGrpcExceptionInterceptor>();
        });
        return services;
    }
}