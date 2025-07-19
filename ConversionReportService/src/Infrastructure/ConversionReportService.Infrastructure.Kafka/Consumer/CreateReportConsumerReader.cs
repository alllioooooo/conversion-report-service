using System.Threading.Channels;
using Confluent.Kafka;
using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Infrastructure.Kafka.Options;
using Microsoft.Extensions.Options;

namespace ConversionReportService.Infrastructure.Kafka.Consumer;

public class CreateReportConsumerReader : IKafkaConsumerMessagesReader<long, CreateReport.Request>
{
    private readonly KafkaConsumerOptions _options;
    private readonly IDeserializer<long> _keyDeserializer;
    private readonly IDeserializer<CreateReport.Request> _valueDeserializer;

    public CreateReportConsumerReader(
        IOptions<KafkaConsumerOptions> options,
        SystemTextJsonSerializer<long> keyDeserializer,
        SystemTextJsonSerializer<CreateReport.Request> valueDeserializer)
    {
        _options = options.Value;
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;
    }

    public async Task ReadMessagesAsync(ChannelWriter<IKafkaConsumerMessage<long, CreateReport.Request>> writer, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<long, CreateReport.Request>(config)
            .SetKeyDeserializer(_keyDeserializer)
            .SetValueDeserializer(_valueDeserializer)
            .Build();

        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken); // todo: заменить логику ожидания создания топика
        consumer.Subscribe(_options.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = consumer.Consume(cancellationToken);
                var message = new KafkaConsumerMessage<long, CreateReport.Request>(consumer, result);
                await writer.WriteAsync(message, cancellationToken);
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}