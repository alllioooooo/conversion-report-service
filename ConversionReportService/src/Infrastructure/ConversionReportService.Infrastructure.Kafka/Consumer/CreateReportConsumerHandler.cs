using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace ConversionReportService.Infrastructure.Kafka.Consumer;

public class CreateReportConsumerHandler : IKafkaConsumerHandler<long, CreateReport.Request>
{
    private readonly IConversionReportService _reportService;
    private readonly ILogger<CreateReportConsumerHandler> _logger;

    public CreateReportConsumerHandler(
        IConversionReportService reportService,
        ILogger<CreateReportConsumerHandler> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public async ValueTask HandleAsync(IEnumerable<IKafkaConsumerMessage<long, CreateReport.Request>> messages, CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            try
            {
                await _reportService.CreateRequestAsync(message.Value, cancellationToken);
                message.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle message {Key}", message.Key);
            }
        }
    }
}