using Confluent.Kafka;
using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Infrastructure.Kafka.Options;
using ConversionReportService.Infrastructure.Kafka.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConversionReportService.Infrastructure.Kafka.Extensions;

public static class KafkaProducerExtensions
{
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KafkaProducerOptions>().BindConfiguration("Kafka:Producer");

        services.AddSingleton<ProducerConfig>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<KafkaProducerOptions>>().Value;

            return new ProducerConfig
            {
                BootstrapServers = options.BootstrapServers,
            };
        });

        services.AddSingleton<IKafkaMessageProducer<TKey, TValue>>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<KafkaProducer<TKey, TValue>>>();
            var config = provider.GetRequiredService<ProducerConfig>();

            var keySerializer = provider.GetService<ISerializer<TKey>>();
            var valueSerializer = provider.GetService<ISerializer<TValue>>();

            var options = provider.GetRequiredService<IOptions<KafkaProducerOptions>>().Value;

            return new KafkaProducer<TKey, TValue>(options.Topic, config, keySerializer, valueSerializer, logger);
        });

        if (typeof(TKey) == typeof(long) && typeof(TValue) == typeof(CreateReport.Request))
        {
            services.AddScoped<IKafkaProducerMessageHandler<CreateReport.Request>, ReportRequestHandler>();
        }

        return services;
    }
}