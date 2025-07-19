namespace ConversionReportService.Infrastructure.Kafka.Consumer;

using Confluent.Kafka;

public class KafkaConsumerMessage<TKey, TValue> : IKafkaConsumerMessage<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly ConsumeResult<TKey, TValue> _result;

    public KafkaConsumerMessage(IConsumer<TKey, TValue> consumer, ConsumeResult<TKey, TValue> result)
    {
        _consumer = consumer;
        _result = result;

        Key = result.Message.Key;
        Value = result.Message.Value;
        Timestamp = new DateTimeOffset(result.Message.Timestamp.UtcDateTime);
        Topic = result.Topic;
        Partition = result.Partition;
        Offset = result.Offset;
    }

    public TKey Key { get; }
    public TValue Value { get; }
    public DateTimeOffset Timestamp { get; }
    public string Topic { get; }
    public Partition Partition { get; }
    public Offset Offset { get; }
    public void Commit()
    {
        _consumer.Commit(_result);
    }
}