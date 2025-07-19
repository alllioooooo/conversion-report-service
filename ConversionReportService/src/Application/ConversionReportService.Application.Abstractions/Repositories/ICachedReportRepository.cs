using ConversionReportService.Application.Abstractions.Dbos;

namespace ConversionReportService.Application.Abstractions.Repositories;

public interface ICachedReportRepository
{
    Task SetAsync(ConversionReportDbo dbo, TimeSpan ttl, CancellationToken cancellationToken);

    Task<ConversionReportDbo?> GetAsync(long registrationId, CancellationToken cancellationToken);
}