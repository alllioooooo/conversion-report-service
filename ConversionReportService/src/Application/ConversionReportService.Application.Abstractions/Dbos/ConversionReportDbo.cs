using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Abstractions.Dbos;

public record ConversionReportDbo
{
    public long Id;
    public long RegistrationId;
    public long ItemId;
    public DateTime? DateFrom;
    public DateTime? DateTo;
    public ReportCreationStatus Status;
    public double? Ratio;
    public long? PayedAmount;
    public DateTime CreatedAt;
}