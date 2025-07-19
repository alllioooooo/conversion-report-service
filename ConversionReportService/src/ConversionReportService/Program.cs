using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Extensions;
using ConversionReportService.Client.Clients;
using ConversionReportService.Client.Extensions;
using ConversionReportService.Client.Middlewares;
using ConversionReportService.Infrastructure.BackgroundServices.Extensions;
using ConversionReportService.Infrastructure.Caching.Extensions;
using ConversionReportService.Infrastructure.Kafka.Extensions;
using ConversionReportService.Infrastructure.Persistence.Extensions;
using ConversionReportService.Infrastructure.RateLimiting.Extensions;
using ConversionReportService.Presentation.Grpc.Extensions;
using ConversionReportService.Presentation.Grpc.Services;
using ConversionReportService.Presentation.Http.Extensions;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddAuthorization();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMigrations();

builder.Services.AddBackgroundMigrationService(builder.Configuration);
builder.Services.AddBackgroundReportStatusCheckupService(builder.Configuration);
builder.Services.AddCaching(builder.Configuration);

builder.Services.AddRedisRateLimiter(builder.Configuration);

builder.Services.AddKafkaConsumer(builder.Configuration);
builder.Services.AddKafkaProducer<long, CreateReport.Request>(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddBackgroundReportProcessingService(builder.Configuration);

builder.Services.AddGrpcServer(builder.Configuration);

builder.Services.AddGrpcClient<ConversionReportService.Grpc.ConversionReportService.ConversionReportServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcOptions:Address"]
                              ?? throw new InvalidOperationException("GrpcOptions:Address is not configured"));
});
builder.Services.AddScoped<ConversionReportGrpcClient>();

builder.Services.AddHttpPresentation();

builder.Services.AddGateway();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGrpcService<ConversionReportGrpcService>();

app.UseMiddleware<ExceptionFormattingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

app.Run();