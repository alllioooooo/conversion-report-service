using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Models.Enums;
using ConversionReportService.Infrastructure.Persistence.Entities;

namespace ConversionReportService.Infrastructure.Persistence.Extensions;

public static class MappingExtensions
{
    public static ItemInteractionDbo ToDbo(this ItemInteractionEntityV1 entity) => new()
    {
        Id = entity.Id,
        ItemId = entity.ItemId,
        Type = Enum.Parse<InteractionType>(entity.Type, ignoreCase: true),
        Timestamp = entity.Timestamp
    };

    public static ItemInteractionEntityV1 ToEntityV1(this ItemInteractionDbo dbo) => new()
    {
        Id = dbo.Id,
        ItemId = dbo.ItemId,
        Type = dbo.Type.ToString(),
        Timestamp = dbo.Timestamp
    };

    public static ConversionReportDbo ToDbo(this ConversionReportEntityV1 entity) => new()
    {
        Id = entity.Id,
        RegistrationId = entity.RegistrationId,
        ItemId = entity.ItemId,
        DateFrom = entity.DateFrom,
        DateTo = entity.DateTo,
        Status = Enum.Parse<ReportCreationStatus>(entity.Status, ignoreCase: true),
        Ratio = entity.Ratio,
        PayedAmount = entity.PayedAmount,
        CreatedAt = entity.CreatedAt
    };

    public static ConversionReportEntityV1 ToEntityV1(this ConversionReportDbo dbo) => new()
    {
        Id = dbo.Id,
        RegistrationId = dbo.RegistrationId,
        ItemId = dbo.ItemId,
        DateFrom = dbo.DateFrom,
        DateTo = dbo.DateTo,
        Status = dbo.Status.ToString(),
        Ratio = dbo.Ratio,
        PayedAmount = dbo.PayedAmount,
        CreatedAt = dbo.CreatedAt
    };
}