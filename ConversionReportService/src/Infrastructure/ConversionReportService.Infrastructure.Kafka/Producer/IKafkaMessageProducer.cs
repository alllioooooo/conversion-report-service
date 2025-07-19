namespace ConversionReportService.Infrastructure.Kafka.Producer;

public interface IKafkaMessageProducer<TKey, TValue>
{
    Task ProduceAsync(KafkaProducerMessage<TKey, TValue> messages, CancellationToken cancellationToken);
}