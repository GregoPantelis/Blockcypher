using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Repository;
using ICMarkets.Blockcypher.Infrastructure.Persistance.UoW;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Infrastructure.UniTests.Persistance.Repos;

public class UserAuthRepositoryTests
{
    private readonly Mock<ILogger<UnitOfWork<UserAuthDbContext>>> _logger;

    public UserAuthRepositoryTests()
    {
        _logger = new Mock<ILogger<UnitOfWork<UserAuthDbContext>>>();
    }

    [Fact]
    public async Task FindBy_UsernameExists_ReturnsCorrectUser()
    {
        // Arrange
        await using var context = GlobalUsings.CreateUserAuthDbContext();

        context.Set<UserModel>().AddRange(
            GlobalUsings.CreateUser("pantelis"),
            GlobalUsings.CreateUser("john"));

        await context.SaveChangesAsync();

        var repository =
            new Repository<UserAuthDbContext, UserModel>(
                context,
                _logger.Object);

        // Act
        var result = repository
            .FindBy(x => x.Username == "pantelis")
            .SingleOrDefault();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pantelis", result.Username);
    }

    [Fact]
    public async Task FindBy_UsernameDoesNotExist_ReturnsEmpty()
    {
        // Arrange
        await using var context = GlobalUsings.CreateUserAuthDbContext();

        context.Set<UserModel>().Add(
            GlobalUsings.CreateUser("pantelis"));

        await context.SaveChangesAsync();

        var repository =
            new Repository<UserAuthDbContext, UserModel>(
                context,
                _logger.Object);

        // Act
        var result = repository
            .FindBy(x => x.Username == "unknown")
            .ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindBy_ActiveUsers_ReturnsOnlyActiveUsers()
    {
        // Arrange
        await using var context = GlobalUsings.CreateUserAuthDbContext();

        context.Set<UserModel>().AddRange(
            GlobalUsings.CreateUser("user1", true),
            GlobalUsings.CreateUser("user2", false),
            GlobalUsings.CreateUser("user3", true));

        await context.SaveChangesAsync();

        var repository =
            new Repository<UserAuthDbContext, UserModel>(
                context,
                _logger.Object);

        // Act
        var result = repository
            .FindBy(x => x.IsActive)
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.True(x.IsActive));
    }

    [Fact]
    public async Task FindBy_UserIdExists_ReturnsUserAuthentication()
    {
        // Arrange
        await using var context = GlobalUsings.CreateUserAuthDbContext();

        context.Set<UserAuthModel>().Add(
            new UserAuthModel
            {
                UserId = 10,
                PasswordHash = "test-password-hash",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UtcCreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var repository =
            new Repository<UserAuthDbContext, UserAuthModel>(
                context,
                _logger.Object);

        // Act
        var result = repository
            .FindBy(x => x.UserId == 10)
            .SingleOrDefault();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.UserId);
        Assert.Equal(
            "test-password-hash",
            result.PasswordHash);
    }
}