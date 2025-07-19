namespace ConversionReportService.Infrastructure.Persistence.Options;

public sealed class PostgresOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}