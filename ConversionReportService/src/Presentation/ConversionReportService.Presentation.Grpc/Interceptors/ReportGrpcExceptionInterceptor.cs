using ConversionReportService.Application.Contracts.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace ConversionReportService.Presentation.Grpc.Interceptors;

public class ReportGrpcExceptionInterceptor : Interceptor
{
    private readonly ILogger<ReportGrpcExceptionInterceptor> _logger;

    public ReportGrpcExceptionInterceptor(ILogger<ReportGrpcExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ReportCancelledException)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Report was cancelled"));
        }
        catch (ReportPendingException)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Report is still pending"));
        }
        catch (ReportProcessingException)
        {
            throw new RpcException(new Status(StatusCode.Unavailable, "Report is still processing"));
        }
        catch (NoItemInformationFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (ReportNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (RpcException rpcEx)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"An unexpected error occurred: {ex.Message}"));
        }
    }
}