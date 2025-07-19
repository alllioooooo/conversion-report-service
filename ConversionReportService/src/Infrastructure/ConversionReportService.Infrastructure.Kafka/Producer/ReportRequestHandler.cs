using ConversionReportService.Application.Contracts.Dtos;
using Microsoft.Extensions.Logging;

namespace ConversionReportService.Infrastructure.Kafka.Producer;

internal sealed class ReportRequestHandler : IKafkaProducerMessageHandler<CreateReport.Request>
{
    private readonly IKafkaMessageProducer<long, CreateReport.Request> _producer;
    private readonly ILogger<ReportRequestHandler> _logger;

    public ReportRequestHandler(
        IKafkaMessageProducer<long, CreateReport.Request> producer,
        ILogger<ReportRequestHandler> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async ValueTask HandleAsync(CreateReport.Request message, CancellationToken cancellationToken)
    {
        var kafkaMessage = new KafkaProducerMessage<long, CreateReport.Request>(
            Key: message.RegistrationId,
            Value: message
        );

        _logger.LogInformation("Producing report creation message: {@Message}", message);

        await _producer.ProduceAsync(kafkaMessage, cancellationToken);
    }
}