using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Infrastructure.Persistence.Entities;

public class ItemInteractionEntityV1
{
    public long Id { get; set; }
    public long ItemId { get; set; }
    public string Type { get; set; }
    public DateTime Timestamp { get; set; }
}