using ConversionReportService.Application.Abstractions.Dbos;
using ConversionReportService.Application.Models.Enums;

namespace ConversionReportService.Application.Abstractions.Repositories;

public interface IReportRepository
{
    Task<long> AddAsync(ConversionReportDbo dbo, CancellationToken cancellationToken);

    Task<ConversionReportDbo?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<ConversionReportDbo> GetByRegistrationIdAsync(long registrationId, CancellationToken cancellationToken);

    Task UpdateStatusAsync(long reportId, ReportCreationStatus newStatus, CancellationToken cancellationToken);

    Task<ConversionReportDbo[]> GetProcessingOlderThanAsync(DateTime olderThanUtc, CancellationToken cancellationToken);

    Task<ConversionReportDbo[]> GetPendingOlderThanAsync(DateTime olderThanUtc, int limit, CancellationToken cancellationToken);

    Task UpdateAsync(ConversionReportDbo dbo, CancellationToken cancellationToken);
}