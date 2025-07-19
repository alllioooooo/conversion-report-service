using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Models;

namespace ConversionReportService.Application.Contracts.Services;

public interface IItemInteractionService
{
    Task<long> AddAsync(AddItemInteraction.Request request, CancellationToken cancellationToken);
    Task<ItemInteraction[]> GetByItemIdAsync(long itemId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
}