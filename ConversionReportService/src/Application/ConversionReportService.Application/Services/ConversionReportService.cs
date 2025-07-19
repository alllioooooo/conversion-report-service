using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Abstractions.Repositories;
using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Contracts.Exceptions;
using ConversionReportService.Application.Contracts.Services;
using ConversionReportService.Application.Models.Enums;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ConversionReportService.Application.Services;

public class ConversionReportService : IConversionReportService
{
    private readonly IItemInteractionService _itemInteractionService;
    private readonly IReportRepository _repository;
    private readonly ICachedReportRepository _cache;
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<ConversionReportService> _logger;

    public ConversionReportService(IItemInteractionService itemInteractionService, IReportRepository repository, ICachedReportRepository cache, NpgsqlDataSource dataSource, ILogger<ConversionReportService> logger)
    {
        _itemInteractionService = itemInteractionService;
        _repository = repository;
        _cache = cache;
        _dataSource = dataSource;
        _logger = logger;
    }

    public async Task<GetReport.Response> GetAsync(GetReport.Request request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(request.RegistrationId, cancellationToken);
        if (cached is not null)
        {
            return new GetReport.Response(
                Status: cached.Status,
                Ratio: cached.Ratio,
                PayedAmount: cached.PayedAmount
            );
        }

        var report = await _repository.GetByRegistrationIdAsync(request.RegistrationId, cancellationToken);

        if (report is null)
            throw new ReportNotFoundException(request.RegistrationId);

        switch (report.Status)
        {
            case ReportCreationStatus.Cancelled:
                throw new ReportCancelledException();
            case ReportCreationStatus.Pending:
                throw new ReportPendingException();
            case ReportCreationStatus.Processing:
                throw new ReportProcessingException();
            case ReportCreationStatus.Done:
                await _cache.SetAsync(report, TimeSpan.FromMinutes(30), cancellationToken);
                return new GetReport.Response(
                    Status: report.Status,
                    Ratio: report.Ratio,
                    PayedAmount: report.PayedAmount
                );
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<bool> CreateRequestAsync(CreateReport.Request request, CancellationToken cancellationToken)
    {
        var dbo = new ConversionReportDbo
        {
            RegistrationId = request.RegistrationId,
            ItemId = request.ItemId,
            DateFrom = request.DateFrom ?? null,
            DateTo = request.DateTo ?? null,
            Status = ReportCreationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var existing = await _repository.GetByRegistrationIdAsync(request.RegistrationId, cancellationToken);

        if (existing is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        await _repository.AddAsync(dbo, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task ProcessReportAsync(long reportId, CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(reportId, cancellationToken);
        if (report is null || report.Status != ReportCreationStatus.Pending)
            return;

        await _repository.UpdateStatusAsync(reportId, ReportCreationStatus.Processing, cancellationToken);

        var interactions = await _itemInteractionService.GetByItemIdAsync(
            report.ItemId,
            report.DateFrom,
            report.DateTo,
            cancellationToken);

        var views = interactions.Count(i => i.Type == InteractionType.Viewed);
        var payments = interactions.Count(i => i.Type == InteractionType.Payed);

        var ratio = views == 0 ? null : (double?)payments / (double)views;

        var updated = report with { Status = ReportCreationStatus.Done, Ratio = ratio, PayedAmount = payments};

        _logger.LogInformation("Processing report: {@Report}", report);
        _logger.LogInformation("Views: {Views}, Payments: {Payments}", views, payments);
        _logger.LogInformation("Calculated Ratio: {Ratio}", ratio);

        await _repository.UpdateAsync(updated, cancellationToken);
    }
}