using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Models;

public record ItemInteraction(
    long Id,
    InteractionType Type
);
