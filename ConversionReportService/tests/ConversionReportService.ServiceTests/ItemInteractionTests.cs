using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Application.Contracts.Exceptions;
using ConversionReportService.Application.Models.Enums;
using ConversionReportService.Application.Services;
using ConversionReportService.ServiceTests.Fixtures;
using Xunit;

namespace ConversionReportService.ServiceTests;

[CollectionDefinition("Postgres only collection")]
public class DatabaseCollection : ICollectionFixture<PostgresFixture>;

[Collection("Postgres only collection")]
public class ItemInteractionTests
{
    private readonly ItemInteractionService _service;

    public ItemInteractionTests(PostgresFixture fixture)
    {
        _service = fixture.Service;
    }

    [Fact]
    public async Task AddAsync_ShouldInsertInteraction_AndReturnId()
    {
        // Arrange
        var request = new AddItemInteraction.Request(
            ItemId: 5252,
            Type: InteractionType.Viewed,
            Timestamp: DateTime.UtcNow);

        // Act
        var id = await _service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.True(id > 0);
    }

    [Fact]
    public async Task GetByItemIdAsync_ShouldReturnInsertedInteraction()
    {
        // Arrange
        var itemId = 525252;
        var request = new AddItemInteraction.Request(
            ItemId: itemId,
            Type: InteractionType.Payed,
            Timestamp: DateTime.UtcNow);

        await _service.AddAsync(request, CancellationToken.None);

        // Act
        var result = await _service.GetByItemIdAsync(itemId, null, null, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(InteractionType.Payed, result[0].Type);
    }

    [Fact]
    public async Task GetByItemIdAsync_ShouldThrow_WhenNoDataExists()
    {
        // Arrange
        var nonexistentItemId = long.MaxValue;

        // Act
        // Assert
        await Assert.ThrowsAsync<NoItemInformationFoundException>(() =>
            _service.GetByItemIdAsync(nonexistentItemId, null, null, CancellationToken.None));
    }
}