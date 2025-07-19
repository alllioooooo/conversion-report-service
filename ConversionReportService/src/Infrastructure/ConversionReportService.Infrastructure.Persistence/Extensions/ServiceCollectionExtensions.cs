using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Application.Models.Enums;
using ConversionReportService.Infrastructure.Persistence.Options;
using ConversionReportService.Infrastructure.Persistence.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ConversionReportService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PostgresOptions>().BindConfiguration("Persistence:Postgres");

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;

            var builder = new NpgsqlDataSourceBuilder(options.ConnectionString);

            builder.MapEnum<InteractionType>("interaction_type");
            builder.MapEnum<ReportCreationStatus>("report_creation_status");

            return builder.Build();
        });
        services.AddScoped<IItemInteractionRepository, ItemInteractionRepository>();
        services.AddScoped<IReportRepository, ConversionReportRepository>();

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;
                    return options.ConnectionString;
                })
                .WithMigrationsIn(typeof(IAssemblyMarker).Assembly));

        return services;
    }

    public static Task RunMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        if (runner.HasMigrationsToApplyUp())
        {
            runner.MigrateUp();
        }

        return Task.CompletedTask;
    }
}