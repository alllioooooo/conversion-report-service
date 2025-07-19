using Confluent.Kafka;

namespace ConversionReportService.Infrastructure.Kafka.Consumer;

public interface IKafkaConsumerMessage<out TKey, out TValue>
{
    TKey Key { get; }
    TValue Value { get; }
    DateTimeOffset Timestamp { get; }
    string Topic { get; }
    Partition Partition { get; }
    Offset Offset { get; }
    void Commit();
}