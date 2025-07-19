namespace ConversionReportService.Application.Contracts.Exceptions;


public class NoItemInformationFoundException : Exception
{
    public NoItemInformationFoundException(long itemId)
        : base($"No item interaction data found for item ID: {itemId}")
    {
    }
}