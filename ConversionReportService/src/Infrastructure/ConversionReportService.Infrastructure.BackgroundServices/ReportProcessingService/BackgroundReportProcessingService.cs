using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConversionReportService.Infrastructure.BackgroundServices.ReportProcessingService;

public class BackgroundReportProcessingService : BackgroundService
{
    private readonly ILogger<BackgroundReportProcessingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ReportProcessingServiceOptions _options;

    public BackgroundReportProcessingService(
        ILogger<BackgroundReportProcessingService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<ReportProcessingServiceOptions> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var reportRepository = scope.ServiceProvider.GetRequiredService<IReportRepository>();
                var reportService = scope.ServiceProvider.GetRequiredService<IConversionReportService>();

                var pendingReports = await reportRepository.GetPendingOlderThanAsync(
                    DateTime.UtcNow,
                    _options.DegreeOfParallelism,
                    stoppingToken);

                if (pendingReports.Length == 0)
                {
                    _logger.LogInformation("No reports to process");
                }
                else
                {
                    _logger.LogInformation("Processing {Count} reports", pendingReports.Length);

                    await Parallel.ForEachAsync(pendingReports.Take(_options.DegreeOfParallelism), stoppingToken, async (report, token) =>
                    {
                        try
                        {
                            await reportService.ProcessReportAsync(report.Id, token);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing report {ReportId}", report.Id);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading reports for processing");
            }

            await Task.Delay(_options.Interval, stoppingToken);
        }
    }
}