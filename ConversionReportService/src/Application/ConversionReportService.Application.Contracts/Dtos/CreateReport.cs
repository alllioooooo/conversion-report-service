namespace ConversionReportService.Application.Contracts.Dtos;

public static class CreateReport
{
    public readonly record struct Request(
        long RegistrationId,
        long ItemId,
        DateTime? DateFrom,
        DateTime? DateTo
    );
}