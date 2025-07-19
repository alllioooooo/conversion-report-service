namespace ConversionReportService.Application.Contracts.Exceptions;

public class ReportCancelledException : Exception
{
    public ReportCancelledException() : base("Report has been cancelled") { }
}
