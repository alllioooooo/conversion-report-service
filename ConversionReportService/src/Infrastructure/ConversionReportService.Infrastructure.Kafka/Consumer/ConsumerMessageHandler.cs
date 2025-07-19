using System.Threading.Channels;
using ConversionReportService.Infrastructure.Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConversionReportService.Infrastructure.Kafka.Consumer;

public class ConsumerMessageHandler<TKey, TValue>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConsumerMessageHandler<TKey, TValue>> _logger;

    public ConsumerMessageHandler(
        IOptions<KafkaConsumerOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<ConsumerMessageHandler<TKey, TValue>> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleAsync(
        ChannelReader<IKafkaConsumerMessage<TKey, TValue>> reader,
        CancellationToken cancellationToken)
    {
        await foreach (var message in reader.ReadAllAsync(cancellationToken))
        {
            if (message.Value is null)
            {
                _logger.LogWarning("Received null message value for key: {Key}", message.Key);
                continue;
            }

            await using var scope = _scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IKafkaConsumerHandler<TKey, TValue>>();

            try
            {
                _logger.LogInformation("Handling message from topic {Topic}, partition {Partition}, offset {Offset}", 
                    message.Topic, message.Partition.Value, message.Offset.Value);

                await handler.HandleAsync([message], cancellationToken);

                message.Commit();

                _logger.LogInformation("Message committed: topic={Topic}, partition={Partition}, offset={Offset}", 
                    message.Topic, message.Partition.Value, message.Offset.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling message from topic={Topic}, partition={Partition}, offset={Offset}", 
                    message.Topic, message.Partition.Value, message.Offset.Value);
            }
        }

        _logger.LogInformation("Stopped consuming Kafka messages.");
    }
}