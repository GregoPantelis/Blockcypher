using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Repository;
using ICMarkets.Blockcypher.Infrastructure.Persistance.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Infrastructure.UnitTests.Persistance.Repos;

public class BlockchainRepositoryTests
{
    private readonly Mock<ILogger<UnitOfWork<BlockcypherDbContext>>> _logger;

    public BlockchainRepositoryTests()
    {
        _logger =
            new Mock<ILogger<UnitOfWork<BlockcypherDbContext>>>();
    }

    [Fact]
    public async Task AddAsync_ValidEntity_ReturnsSuccess()
    {
        // Arrange
        await using var context = GlobalUsings.CreateBlockcypherDbContext();

        var repository =
            new Repository<BlockcypherDbContext, BlockchainModel>(
                context,
                _logger.Object);

        var entity = GlobalUsings.CreateBlockchainModel();

        // Act
        var result = await repository.AddAsync(entity);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
        Assert.Equal(
            OperationResults.Common.Successful,
            result.Result);
    }

    [Fact]
    public async Task AddAsync_ValidEntity_AddsEntityToContext()
    {
        // Arrange
        await using var context = GlobalUsings.CreateBlockcypherDbContext();

        var repository =
            new Repository<BlockcypherDbContext, BlockchainModel>(
                context,
                _logger.Object);

        var entity = GlobalUsings.CreateBlockchainModel();

        // Act
        await repository.AddAsync(entity);

        // Assert
        Assert.Equal(
            EntityState.Added,
            context.Entry(entity).State);
    }

    [Fact]
    public async Task GetAll_EntitiesExist_ReturnsAllEntities()
    {
        // Arrange
        await using var context = GlobalUsings.CreateBlockcypherDbContext();

        context.Set<BlockchainModel>().AddRange(GlobalUsings.CreateBlockchainModel("btc", "main"), GlobalUsings.CreateBlockchainModel("eth", "main"));

        await context.SaveChangesAsync();

        var repository =
            new Repository<BlockcypherDbContext, BlockchainModel>(
                context,
                _logger.Object);

        // Act
        var result = repository
            .GetAll()
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task FindBy_MatchingEntityExists_ReturnsMatchingEntities()
    {
        // Arrange
        await using var context = GlobalUsings.CreateBlockcypherDbContext();

        context.Set<BlockchainModel>().AddRange(GlobalUsings.CreateBlockchainModel("btc", "main"), GlobalUsings.CreateBlockchainModel("btc", "test"), GlobalUsings.CreateBlockchainModel("eth", "main"));

        await context.SaveChangesAsync();

        var repository =
            new Repository<BlockcypherDbContext, BlockchainModel>(
                context,
                _logger.Object);

        // Act
        var result = repository
            .FindBy(x =>
                x.Coin == "btc" &&
                x.Chain == "main")
            .ToList();

        // Assert
        Assert.Single(result);

        Assert.Equal(
            "btc",
            result[0].Coin);

        Assert.Equal(
            "main",
            result[0].Chain);
    }
}