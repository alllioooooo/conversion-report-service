namespace ConversionReportService.Application.Contracts.Exceptions;

public class ReportPendingException : Exception
{
    public ReportPendingException() : base("Report is pending") { }
}