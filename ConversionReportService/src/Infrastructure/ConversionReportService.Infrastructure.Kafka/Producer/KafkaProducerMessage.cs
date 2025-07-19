namespace ConversionReportService.Infrastructure.Kafka.Producer;

public record KafkaProducerMessage<TKey, TValue>(TKey Key, TValue Value);