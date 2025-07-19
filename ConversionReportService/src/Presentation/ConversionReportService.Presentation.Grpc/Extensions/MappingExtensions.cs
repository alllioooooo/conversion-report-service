using ApplicationStatus = ConversionReportService.Application.Models.Enums.ReportCreationStatus;
using GrpcStatus = ConversionReportService.Grpc.ReportCreationStatus;

namespace ConversionReportService.Presentation.Grpc.Extensions;

public static class MappingExtensions
{
    public static GrpcStatus ToGrpc(this ApplicationStatus status) => status switch
    {
        ApplicationStatus.Pending => GrpcStatus.Pending,
        ApplicationStatus.Processing => GrpcStatus.Processing,
        ApplicationStatus.Done => GrpcStatus.Done,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static ApplicationStatus ToApplication(this GrpcStatus status) => status switch
    {
        GrpcStatus.Pending => ApplicationStatus.Pending,
        GrpcStatus.Processing => ApplicationStatus.Processing,
        GrpcStatus.Done => ApplicationStatus.Done,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };
}