using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Infrastructure.Persistence.Entities;
using ConversionReportService.Infrastructure.Persistence.Extensions;
using Dapper;
using Npgsql;

namespace ConversionReportService.Infrastructure.Persistence.Repositories;

public class ItemInteractionRepository : IItemInteractionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ItemInteractionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> AddAsync(ItemInteractionDbo dbo, CancellationToken cancellationToken)
    {
        const string sql = @"
            insert into item_interactions (item_id, type, timestamp)
            values (@ItemId, @Type::interaction_type, @Timestamp)
            returning id;
        ";

        var entity = dbo.ToEntityV1();

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task<ItemInteractionDbo[]> GetByItemIdAsync(long itemId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var sql = @"
            select id, item_id, type::text as type, timestamp
            from item_interactions
            where item_id = @ItemId
        ";

        if (from.HasValue)
            sql += " and timestamp >= @From";
        if (to.HasValue)
            sql += " and timestamp <= @To";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var entities = await connection.QueryAsync<ItemInteractionEntityV1>(sql, new { ItemId = itemId, From = from, To = to });

        return entities
            .Select(e => e.ToDbo())
            .ToArray();
    }
}