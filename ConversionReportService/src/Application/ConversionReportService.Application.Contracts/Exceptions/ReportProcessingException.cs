namespace ConversionReportService.Application.Contracts.Exceptions;

public class ReportProcessingException : Exception
{
    public ReportProcessingException() : base("Report is still processing") { }
}