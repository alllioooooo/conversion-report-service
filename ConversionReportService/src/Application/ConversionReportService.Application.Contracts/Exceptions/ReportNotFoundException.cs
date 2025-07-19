namespace ConversionReportService.Application.Contracts.Exceptions;

public class ReportNotFoundException : Exception
{
    public ReportNotFoundException(long registrationId)
        : base($"Report with registration ID {registrationId} not found.")
    {
    }
}