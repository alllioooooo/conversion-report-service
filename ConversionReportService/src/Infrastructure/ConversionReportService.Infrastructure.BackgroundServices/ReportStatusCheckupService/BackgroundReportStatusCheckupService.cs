using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Application.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConversionReportService.Infrastructure.BackgroundServices.ReportStatusCheckupService;

public class BackgroundReportStatusCheckupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ReportStatusCheckupOptions _options;
    private readonly ILogger<BackgroundReportStatusCheckupService> _logger;

    public BackgroundReportStatusCheckupService(
        IServiceProvider serviceProvider,
        IOptions<ReportStatusCheckupOptions> options,
        ILogger<BackgroundReportStatusCheckupService> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Report status checkup background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var reportRepository = scope.ServiceProvider.GetRequiredService<IReportRepository>();

                var stuckTime = DateTime.UtcNow - _options.MaxProcessingTimeSpan;

                var stuckReports = await reportRepository.GetProcessingOlderThanAsync(stuckTime, stoppingToken);

                if (stuckReports.Length == 0)
                {
                    _logger.LogInformation("No stuck reports found at {Time}", DateTime.UtcNow);
                }
                else
                {
                    _logger.LogWarning("Found {Count} stuck reports", stuckReports.Length);
                }

                foreach (var report in stuckReports)
                {
                    _logger.LogWarning("Cancelling report with ID {ReportId}", report.Id);
                    await reportRepository.UpdateStatusAsync(report.Id, ReportCreationStatus.Cancelled, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred during report status checkup");
            }

            await Task.Delay(_options.IntervalTimeSpan, stoppingToken);
        }
    }
}