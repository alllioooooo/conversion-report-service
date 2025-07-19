using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace ConversionReportService.Infrastructure.Kafka.Producer;

internal class KafkaProducer<TKey, TValue> : IKafkaMessageProducer<TKey, TValue>, IDisposable
{
    private readonly ILogger<KafkaProducer<TKey, TValue>> _logger;
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topicName;

    public KafkaProducer(
        string topicName,
        ProducerConfig producerConfig,
        ISerializer<TKey>? keySerializer,
        ISerializer<TValue>? valueSerializer,
        ILogger<KafkaProducer<TKey, TValue>> logger)
    {
        _logger = logger;
        _topicName = topicName;

        _producer = new ProducerBuilder<TKey, TValue>(producerConfig)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(KafkaProducerMessage<TKey, TValue> message, CancellationToken cancellationToken)
    {
        try
        {
            var kafkaMessage = new Message<TKey, TValue>
            {
                Key = message.Key,
                Value = message.Value,
            };

            await _producer.ProduceAsync(_topicName, kafkaMessage, cancellationToken);

            _logger.LogInformation(
                "Message successfully produced to topic {_topicName}", _topicName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error producing in topic {_topicName}", _topicName);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}