using ConversionReportService.Application.Contracts.Dtos;

namespace ConversionReportService.Application.Contracts.Services;

public interface IConversionReportService
{
    Task<GetReport.Response> GetAsync(GetReport.Request request, CancellationToken cancellationToken);

    public Task<bool> CreateRequestAsync(CreateReport.Request request, CancellationToken cancellationToken);

    Task ProcessReportAsync(long reportId, CancellationToken cancellationToken);
}