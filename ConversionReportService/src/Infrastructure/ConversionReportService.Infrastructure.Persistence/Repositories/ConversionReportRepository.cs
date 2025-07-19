using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Application.Models.Enums;
using ConversionReportService.Infrastructure.Persistence.Entities;
using ConversionReportService.Infrastructure.Persistence.Extensions;
using Dapper;
using Npgsql;

namespace ConversionReportService.Infrastructure.Persistence.Repositories;

public class ConversionReportRepository : IReportRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ConversionReportRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> AddAsync(ConversionReportDbo dbo, CancellationToken cancellationToken)
    {
        const string sql = @"
            insert into conversion_reports (
                registration_id, item_id, date_from, date_to, status, ratio, payed_amount, created_at
            )
            values (
                @RegistrationId, @ItemId, @DateFrom, @DateTo, @Status::report_creation_status, @Ratio, @PayedAmount, @CreatedAt
            )
            returning id;
        ";

        var entity = dbo.ToEntityV1();

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task<ConversionReportDbo?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = @"
            select id, registration_id, item_id, date_from, date_to, status::text as status, ratio, payed_amount, created_at
            from conversion_reports
            where id = @Id;
        ";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var entity = await connection.QuerySingleOrDefaultAsync<ConversionReportEntityV1>(sql, new { Id = id });
        return entity?.ToDbo();
    }

    public async Task<ConversionReportDbo> GetByRegistrationIdAsync(long registrationId, CancellationToken cancellationToken)
    {
        const string sql = @"
            select id, registration_id, item_id, date_from, date_to, status::text as status, ratio, payed_amount, created_at
            from conversion_reports
            where registration_id = @RegistrationId;
        ";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var entity = await connection.QuerySingleOrDefaultAsync<ConversionReportEntityV1>(sql, new { RegistrationId = registrationId });
        return entity?.ToDbo();
    }

    public async Task UpdateStatusAsync(long reportId, ReportCreationStatus newStatus, CancellationToken cancellationToken)
    {
        const string sql = @"
            update conversion_reports 
            set status = @Status::report_creation_status
            where registration_id = @RegistrationId;
        ";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(sql, new { RegistrationId = reportId, Status = newStatus.ToString() });
    }

    public async Task<ConversionReportDbo[]> GetProcessingOlderThanAsync(DateTime olderThanUtc, CancellationToken cancellationToken)
    {
        const string sql = @"
            select id, registration_id, item_id, date_from, date_to, status::text as status, ratio, payed_amount, created_at
            from conversion_reports
            where status = @Status::report_creation_status and created_at < @OlderThan;
        ";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var entities = await connection.QueryAsync<ConversionReportEntityV1>(sql, new
        {
            Status = nameof(ReportCreationStatus.Processing),
            OlderThan = olderThanUtc
        });

        return entities.Select(e => e.ToDbo()).ToArray();
    }

    public async Task<ConversionReportDbo[]> GetPendingOlderThanAsync(DateTime olderThanUtc, int limit, CancellationToken cancellationToken)
    {
        const string sql = @"
            select id, registration_id, item_id, date_from, date_to, status::text as status, ratio, payed_amount, created_at
            from conversion_reports
            where status = @Status::report_creation_status and created_at < @OlderThan
            order by created_at
            limit @Limit;
        ";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var entities = await connection.QueryAsync<ConversionReportEntityV1>(sql, new
        {
            Status = nameof(ReportCreationStatus.Pending),
            OlderThan = olderThanUtc,
            Limit = limit
        });

        return entities.Select(e => e.ToDbo()).ToArray();
    }

    public async Task UpdateAsync(ConversionReportDbo dbo, CancellationToken cancellationToken)
    {
        const string sql = @"
            update conversion_reports
            set 
                status = @Status::report_creation_status,
                ratio = @Ratio,
                payed_amount = @PayedAmount
            where id = @Id;
        ";

        var entity = dbo.ToEntityV1();

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(sql, entity);
    }
}