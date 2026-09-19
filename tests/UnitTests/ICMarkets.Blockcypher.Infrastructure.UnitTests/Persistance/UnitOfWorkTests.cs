using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using ICMarkets.Blockcypher.Infrastructure.Persistance.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Infrastructure.Tests.Persistance.UoW;

public class UnitOfWorkTests
{
    private readonly Mock<ILogger<UnitOfWork<BlockcypherDbContext>>> _logger;
    private readonly Mock<ILogger<UnitOfWork<ThrowingDbContext>>> _throwLogger;

    public UnitOfWorkTests()
    {
        _logger = new Mock<ILogger<UnitOfWork<BlockcypherDbContext>>>();
        _throwLogger = new Mock<ILogger<UnitOfWork<ThrowingDbContext>>>();
    }

    [Fact]
    public void Repository_FirstCall_ReturnsRepository()
    {
        
        using var context = GlobalUsings.CreateBlockcypherDbContext();

        var unitOfWork =
            new UnitOfWork<BlockcypherDbContext>(
                context,
                _logger.Object);

        
        var repository =
            unitOfWork.Repository<BlockchainModel>();

        
        Assert.NotNull(repository);
    }

    [Fact]
    public void Repository_CalledTwice_ReturnsSameRepositoryInstance()
    {
        
        using var context = GlobalUsings.CreateBlockcypherDbContext();

        var unitOfWork =
            new UnitOfWork<BlockcypherDbContext>(
                context,
                _logger.Object);

        
        var repository1 =
            unitOfWork.Repository<BlockchainModel>();

        var repository2 =
            unitOfWork.Repository<BlockchainModel>();

        
        Assert.Same(repository1, repository2);
    }

    [Fact]
    public async Task CompleteAsync_Success_ReturnsSuccess()
    {
        
        await using var context = GlobalUsings.CreateBlockcypherDbContext();

        var unitOfWork =
            new UnitOfWork<BlockcypherDbContext>(
                context,
                _logger.Object);

        var repository =
            unitOfWork.Repository<BlockchainModel>();

        await repository.AddAsync(
            GlobalUsings.CreateBlockchainModel());

        
        var result = await unitOfWork.CompleteAsync();

        
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);

        Assert.Equal(
            OperationResults.Common.Successful,
            result.Result);
    }

    [Fact]
    public async Task CompleteAsync_Exception_ReturnsInternalServerError()
    {
        var options =
            new DbContextOptionsBuilder<ThrowingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        var contextMock =
            new ThrowingDbContext(options);

        await using var context = new ThrowingDbContext(options);
        
        var unitOfWork =
            new UnitOfWork<ThrowingDbContext>(
                contextMock,
                _throwLogger.Object);

        
        var result = await unitOfWork.CompleteAsync();
        
        Assert.False(result.IsSuccessful);
        Assert.False(result.Data);

        Assert.Equal(
            OperationResults.Common.InternalServerError,
            result.Result);
    }
    
    public  sealed class ThrowingDbContext : CustomContext
    {
        public ThrowingDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new Exception("Database failure");
        }
    }
}