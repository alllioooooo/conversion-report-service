using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Contracts.Dtos;

public static class AddItemInteraction
{
    public readonly record struct Request(
        long ItemId,
        InteractionType Type,
        DateTime Timestamp
    );
}