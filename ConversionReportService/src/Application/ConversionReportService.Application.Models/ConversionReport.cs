using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Models;

public record ConversionReport(
    long ItemId,
    long RegistrationId,
    DateTime? DateFrom,
    DateTime? DateTo,
    ReportCreationStatus Status,
    double? Ratio,
    long? PayedAmount
);