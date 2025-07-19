using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Contracts.Dtos;

public static class GetReport
{
    public readonly record struct Request(long RegistrationId);

    public readonly record struct Response(
        ReportCreationStatus Status,
        double? Ratio,
        long? PayedAmount
    );
}