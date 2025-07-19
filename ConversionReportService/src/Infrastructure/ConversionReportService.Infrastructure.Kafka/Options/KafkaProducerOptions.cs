namespace ConversionReportService.Infrastructure.Kafka.Options;

public sealed class KafkaProducerOptions
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}