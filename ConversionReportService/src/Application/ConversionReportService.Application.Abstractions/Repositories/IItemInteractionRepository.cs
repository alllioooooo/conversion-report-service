using ConversionReportService.Application.Abstractions.Dbos;

namespace ConversionReportService.Application.Abstractions.Repositories;

public interface IItemInteractionRepository
{
    Task<long> AddAsync(ItemInteractionDbo dbo, CancellationToken cancellationToken);

    Task<ItemInteractionDbo[]> GetByItemIdAsync(long itemId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
}