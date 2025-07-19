using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Contracts.Exceptions;
using ConversionReportService.Application.Contracts.Services;
using ConversionReportService.Application.Models;
using Npgsql;

namespace ConversionReportService.Application.Services;

public class ItemInteractionService : IItemInteractionService
{
    private readonly IItemInteractionRepository _repository;
    private readonly NpgsqlDataSource _dataSource;

    public ItemInteractionService(IItemInteractionRepository repository, NpgsqlDataSource dataSource)
    {
        _repository = repository;
        _dataSource = dataSource;
    }

    public async Task<long> AddAsync(AddItemInteraction.Request request, CancellationToken cancellationToken)
    {
        var dbo = new ItemInteractionDbo
        {
            ItemId = request.ItemId,
            Type = request.Type,
            Timestamp = request.Timestamp
        };

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var id = await _repository.AddAsync(dbo, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return id;
    }

    public async Task<ItemInteraction[]> GetByItemIdAsync(long itemId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var result = await _repository.GetByItemIdAsync(itemId, from, to, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        if (result.Length == 0)
            throw new NoItemInformationFoundException(itemId);

        return result
            .Select(x => new ItemInteraction(
                Id: x.Id,
                Type: x.Type
            ))
            .ToArray();
    }
}