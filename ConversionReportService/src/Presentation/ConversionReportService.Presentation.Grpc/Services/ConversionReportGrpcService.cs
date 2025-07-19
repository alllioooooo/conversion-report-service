using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Contracts.Services;
using ConversionReportService.Grpc;
using ConversionReportService.Presentation.Grpc.Extensions;
using Grpc.Core;

namespace ConversionReportService.Presentation.Grpc.Services;

public class ConversionReportGrpcService : ConversionReportService.Grpc.ConversionReportService.ConversionReportServiceBase
{
    private readonly IConversionReportService _conversionReportService;

    public ConversionReportGrpcService(IConversionReportService conversionReportService)
    {
        _conversionReportService = conversionReportService;
    }

    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        var internalRequest = new GetReport.Request(request.RegistrationId);

        var internalResponse = await _conversionReportService.GetAsync(internalRequest, context.CancellationToken);

        return new GetReportResponse
        {
            Status = internalResponse.Status.ToGrpc(),
            Ratio = internalResponse.Ratio ?? 0,
            PayedAmount = internalResponse.PayedAmount ?? 0
        };
    }
}