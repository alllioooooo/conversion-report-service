namespace ConversionReportService.Infrastructure.Kafka.Producer;

public interface IKafkaProducerMessageHandler<in TMessage>
{
    ValueTask HandleAsync(TMessage message, CancellationToken cancellationToken);
}