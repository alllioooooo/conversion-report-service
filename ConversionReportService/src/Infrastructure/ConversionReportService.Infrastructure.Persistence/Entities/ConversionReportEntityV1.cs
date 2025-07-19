using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Infrastructure.Persistence.Entities;

public record ConversionReportEntityV1
{
    public long Id { get; set; }
    public long RegistrationId { get; set; }
    public long ItemId { get; set; }
    public DateTime? DateFrom { get; set; } = null;
    public DateTime? DateTo { get; set; } = null;
    public string Status { get; set; }
    public double? Ratio { get; set; } = null;
    public long? PayedAmount { get; set; } = null;
    public DateTime CreatedAt { get; set; }
}