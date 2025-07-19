using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Contracts.Exceptions;
using ConversionReportService.Application.Models.Enums;
using ConversionReportService.Application.Services;
using ConversionReportService.Infrastructure.Caching.RedisRepositories;
using ConversionReportService.Infrastructure.Persistence.Repositories;
using ConversionReportService.ServiceTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConversionReportService.ServiceTests;

[CollectionDefinition("Postgres and Redis integration collection")]
public class FullIntegrationCollection : ICollectionFixture<PostgresFixture>, ICollectionFixture<RedisFixture> { }

[Collection("Postgres and Redis integration collection")]
public class ConversionReportTests
{
    private readonly Application.Services.ConversionReportService _service;
    private readonly ItemInteractionService _itemInteractionService;
    private readonly ConversionReportRepository _reportRepository;

    public ConversionReportTests(PostgresFixture postgres, RedisFixture redis)
    {
        var reportRepository = new ConversionReportRepository(postgres.DataSource);
        var cache = new CachedReportRepository(redis.GetOptions());
        _itemInteractionService = postgres.Service;
        _reportRepository = new ConversionReportRepository(postgres.DataSource);

        _service = new Application.Services.ConversionReportService(
            _itemInteractionService,
            _reportRepository,
            cache,
            postgres.DataSource,
            NullLogger<Application.Services.ConversionReportService>.Instance
        );
    }

    [Fact]
    public async Task CreateRequestAsync_ShouldSucceed_WhenNewReport()
    {
        // Arrange
        var request = new CreateReport.Request(
            RegistrationId: 993,
            ItemId: 339,
            DateFrom: null,
            DateTo: null
        );

        // Act
        var result = await _service.CreateRequestAsync(request, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRequestAsync_ShouldReturnFalse_WhenDuplicate()
    {
        // Arrange
        var request = new CreateReport.Request(
            RegistrationId: 1981,
            ItemId: 1981,
            DateFrom: null,
            DateTo: null
        );

        await _service.CreateRequestAsync(request, CancellationToken.None);

        // Act
        var result = await _service.CreateRequestAsync(request, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenReportNotExists()
    {
        // Arrange
        var request = new GetReport.Request(RegistrationId: long.MaxValue);

        // Act
        // Assert
        await Assert.ThrowsAsync<ReportNotFoundException>(() => _service.GetAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenStatusIsPending()
    {
        // Arrange
        var registrationId = 1;
        var itemId = 1;

        await _service.CreateRequestAsync(new CreateReport.Request(registrationId, itemId, null, null), CancellationToken.None);

        // Act
        // Assert
        await Assert.ThrowsAsync<ReportPendingException>(() =>
            _service.GetAsync(new GetReport.Request(registrationId), CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenStatusIsProcessing()
    {
        // Arrange
        var registrationId = 2;
        var itemId = 2;

        await _service.CreateRequestAsync(new CreateReport.Request(registrationId, itemId, null, null), CancellationToken.None);

        var report = await _reportRepository.GetByRegistrationIdAsync(registrationId, CancellationToken.None);
        var updated = report with { Status = ReportCreationStatus.Processing };
        await _reportRepository.UpdateAsync(updated, CancellationToken.None);

        // Act
        // Assert
        await Assert.ThrowsAsync<ReportProcessingException>(() =>
            _service.GetAsync(new GetReport.Request(registrationId), CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenStatusIsCancelled()
    {
        // Arrange
        var registrationId = 3;
        var itemId = 3;

        await _service.CreateRequestAsync(new CreateReport.Request(registrationId, itemId, null, null), CancellationToken.None);

        var report = await _reportRepository.GetByRegistrationIdAsync(registrationId, CancellationToken.None);
        var updated = report with { Status = ReportCreationStatus.Cancelled };
        await _reportRepository.UpdateAsync(updated, CancellationToken.None);

        // Act
        // Assert
        await Assert.ThrowsAsync<ReportCancelledException>(() =>
            _service.GetAsync(new GetReport.Request(registrationId), CancellationToken.None));
    }

}