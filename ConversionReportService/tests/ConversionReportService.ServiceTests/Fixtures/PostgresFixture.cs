using ConversionReportService.Application.Services;
using ConversionReportService.Infrastructure.Persistence.Migrations;
using ConversionReportService.Infrastructure.Persistence.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ConversionReportService.ServiceTests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private NpgsqlDataSource _dataSource = null!;
    private ItemInteractionService _service = null!;
    private ConversionReportRepository _convRepository = null!;

    public PostgresFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("test_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var connectionString = _postgresContainer.GetConnectionString();
        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(InitSchema).Assembly).For.Migrations())
            .AddLogging()
            .BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        var repository = new ItemInteractionRepository(_dataSource);
        _convRepository = new ConversionReportRepository(_dataSource);
        _service = new ItemInteractionService(repository, _dataSource);
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public NpgsqlDataSource DataSource => _dataSource;
    public ItemInteractionService Service => _service;

    public ConversionReportRepository ConversionReportRepository => _convRepository;

}