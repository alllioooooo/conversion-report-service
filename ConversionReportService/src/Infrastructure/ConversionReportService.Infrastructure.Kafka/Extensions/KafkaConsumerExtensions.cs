using Confluent.Kafka;
using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Infrastructure.Kafka.Consumer;
using ConversionReportService.Infrastructure.Kafka.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConversionReportService.Infrastructure.Kafka.Extensions;

public static class KafkaConsumerExtensions
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<KafkaConsumerOptions>().BindConfiguration("Kafka:Consumer");

        services.AddSingleton<SystemTextJsonSerializer<long>>();
        services.AddSingleton<ISerializer<long>>(provider => provider.GetRequiredService<SystemTextJsonSerializer<long>>());
        services.AddSingleton<IDeserializer<long>>(provider => provider.GetRequiredService<SystemTextJsonSerializer<long>>());

        services.AddSingleton<SystemTextJsonSerializer<CreateReport.Request>>();
        services.AddSingleton<ISerializer<CreateReport.Request>>(provider => provider.GetRequiredService<SystemTextJsonSerializer<CreateReport.Request>>());
        services.AddSingleton<IDeserializer<CreateReport.Request>>(provider => provider.GetRequiredService<SystemTextJsonSerializer<CreateReport.Request>>());

        services.AddSingleton<IKafkaConsumerMessagesReader<long, CreateReport.Request>, CreateReportConsumerReader>();
        services.AddScoped<IKafkaConsumerHandler<long, CreateReport.Request>, CreateReportConsumerHandler>();

        services.AddSingleton<ConsumerMessageHandler<long, CreateReport.Request>>();
        services.AddHostedService<BackgroundKafkaConsumerService<long, CreateReport.Request>>();

        return services;
    }
}