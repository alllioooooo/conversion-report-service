using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Presentation.Http.Models;

public class AddItemInteractionRequest
{
    public long ItemId { get; init; }
    public InteractionType Type { get; init; }
    public DateTime Timestamp { get; init; }
}