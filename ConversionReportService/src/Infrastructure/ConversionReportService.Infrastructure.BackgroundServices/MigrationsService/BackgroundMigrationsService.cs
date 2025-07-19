using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace ConversionReportService.Infrastructure.BackgroundServices.MigrationsService;

public class BackgroundMigrationsService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundMigrationsService> _logger;

    public BackgroundMigrationsService(IServiceProvider serviceProvider, ILogger<BackgroundMigrationsService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartMigrationsAsync();
        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = stoppingToken;
        return Task.CompletedTask;
    }

    private async Task StartMigrationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        if (migrationRunner.HasMigrationsToApplyUp())
        {
            migrationRunner.MigrateUp();
        }

        await Task.CompletedTask;
    }
}