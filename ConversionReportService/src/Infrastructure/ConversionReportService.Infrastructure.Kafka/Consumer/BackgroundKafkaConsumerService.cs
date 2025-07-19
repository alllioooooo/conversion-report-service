using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConversionReportService.Infrastructure.Kafka.Consumer;

public class BackgroundKafkaConsumerService<TKey, TValue> : BackgroundService
{
    private readonly IKafkaConsumerMessagesReader<TKey, TValue> _reader;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BackgroundKafkaConsumerService<TKey, TValue>> _logger;

    public BackgroundKafkaConsumerService(
        IKafkaConsumerMessagesReader<TKey, TValue> reader,
        IServiceScopeFactory scopeFactory,
        ILogger<BackgroundKafkaConsumerService<TKey, TValue>> logger)
    {
        _reader = reader;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Kafka consumer background service.");

        var channel = Channel.CreateBounded<IKafkaConsumerMessage<TKey, TValue>>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            SingleWriter = true,
        });

        var writerTask = Task.Run(
            async () =>
            {
                try
                {
                    await _reader.ReadMessagesAsync(channel.Writer, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while reading messages.");
                }
                finally
                {
                    channel.Writer.Complete();
                }
            },
            stoppingToken);

        var readerTask = Task.Run(
            async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var handler = scope.ServiceProvider.GetRequiredService<ConsumerMessageHandler<TKey, TValue>>();

                    await handler.HandleAsync(channel.Reader, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while handling messages.");
                }
            },
            stoppingToken);

        await Task.WhenAll(writerTask, readerTask);

        _logger.LogInformation("Kafka consumer background service stopped.");
    }
}