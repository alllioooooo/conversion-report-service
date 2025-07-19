using System.Threading.Channels;

namespace ConversionReportService.Infrastructure.Kafka.Consumer;

public interface IKafkaConsumerMessagesReader<TKey, TValue>
{
    Task ReadMessagesAsync(ChannelWriter<IKafkaConsumerMessage<TKey, TValue>> writer, CancellationToken cancellationToken);
}