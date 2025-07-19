using ConversionReportService.Grpc;
using GrpcConversionClient = ConversionReportService.Grpc.ConversionReportService;

namespace ConversionReportService.Client.Clients;

public class ConversionReportGrpcClient
{
    private readonly GrpcConversionClient.ConversionReportServiceClient _client;

    public ConversionReportGrpcClient(GrpcConversionClient.ConversionReportServiceClient client)
    {
        _client = client;
    }

    public async Task<GetReportResponse> GetReportAsync(long registrationId, CancellationToken cancellationToken)
    {
        return await _client.GetReportAsync(new GetReportRequest
        {
            RegistrationId = registrationId
        }, cancellationToken: cancellationToken);
    }
} 