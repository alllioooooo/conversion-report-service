namespace ConversionReportService.Application.Abstractions;

public interface IRateLimiter
{
    Task<bool> AllowAsync(string clientKey, CancellationToken cancellationToken = default);
}