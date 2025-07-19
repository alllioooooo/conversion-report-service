namespace ConversionReportService.Infrastructure.BackgroundServices.ReportProcessingService;

public sealed class ReportProcessingServiceOptions
{
    public int DegreeOfParallelism { get; set; } = 4;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(60);
}