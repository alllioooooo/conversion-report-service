using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Abstractions.Dbos;

public record ItemInteractionDbo
{
    public long Id;
    public long ItemId;
    public InteractionType Type;
    public DateTime Timestamp;
}