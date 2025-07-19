namespace ConversionReportService.Infrastructure.BackgroundServices.ReportStatusCheckupService;

public sealed class ReportStatusCheckupOptions
{
    public TimeSpan IntervalTimeSpan { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan MaxProcessingTimeSpan { get; set; } = TimeSpan.FromHours(24);
}