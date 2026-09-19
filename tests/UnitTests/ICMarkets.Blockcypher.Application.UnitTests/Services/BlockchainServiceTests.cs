using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Application.Services.Blockchain;
using Microsoft.Extensions.Logging;
using ICMarkets.Blockcypher.Application.Services.Authentication;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Domain.Types.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Application.UnitTests.Services;

public class BlockchainServiceTests
{
    private readonly Mock<ILogger<BlockchainService>> _logger;
    private readonly Mock<IBlockchainDataProvider> _provider;
    private readonly Mock<IBlockcypherUnitOfWork> _unitOfWork;
    
    private BlockchainService _blockchainService;
    
    public BlockchainServiceTests()
    {
        _logger = new Mock<ILogger<BlockchainService>>();
        _provider = new Mock<IBlockchainDataProvider>();
        _unitOfWork = new Mock<IBlockcypherUnitOfWork>();
        
        _blockchainService = new BlockchainService(
            _logger.Object,
            _provider.Object,
            _unitOfWork.Object);
    }
    
    private void SetupSuccessfulProvider(
        BlockchainDataFilter filter,
        BlockchainData data)
    {
        _provider
            .Setup(x => x.GetBlockchainData(filter))
            .ReturnsAsync(new OperationResult<BlockchainData>(
                OperationResults.Common.Successful,
                data));
    }
    
    private void SetupSuccessfulAdd(BlockchainData data)
    {
        _unitOfWork
            .Setup(x => x.AddBlockchainAsync(data, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<bool>(
                OperationResults.Common.Successful,
                true));
    }

    #region  CaptureBlockchainDataAsync()

    [Fact]
    public async Task CaptureBlockchainDataAsync_ValidRequest_ProviderReturnsData_SavesAndReturnsSuccess()
    {
        var filter = GlobalUsings.CreateFilter();
        var blockchainData = GlobalUsings.CreateBlockchainData();
        
        SetupSuccessfulProvider(filter, blockchainData);
        SetupSuccessfulAdd(blockchainData);

        _unitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(new OperationResult<bool>(
                OperationResults.Common.Successful,
                true));
        
        OperationResult<BlockchainData> result = await _blockchainService.CaptureBlockchainDataAsync(filter);
        
        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
        Assert.Same(blockchainData, result.Data);

        _provider.Verify(
            x => x.GetBlockchainData(filter),
            Times.Once);

        _unitOfWork.Verify(
            x => x.AddBlockchainAsync(blockchainData, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CompleteAsync(),
            Times.Once);
    }
    
    [Fact]
    public async Task CaptureBlockchainDataAsync_ProviderReturnsNoData_DoesNotSave_ReturnsFailure()
    {
        var filter = GlobalUsings.CreateFilter();

        _provider
            .Setup(x => x.GetBlockchainData(filter))
            .ReturnsAsync(new OperationResult<BlockchainData>(
                OperationResults.Common.InternalServerError,
                null));

        // Act
        var result = await _blockchainService.CaptureBlockchainDataAsync(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(
            OperationResults.Common.InternalServerError,
            result.Result);

        _unitOfWork.Verify(
            x => x.AddBlockchainAsync(It.IsAny<BlockchainData>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.CompleteAsync(),
            Times.Never);
    }

    [Fact]
    public async Task CaptureBlockchainDataAsync_ProviderThrowsException_DoesNotSave_ReturnsInternalServerError()
    {
        var filter = GlobalUsings.CreateFilter();

        _provider
            .Setup(x => x.GetBlockchainData(filter))
            .ThrowsAsync(new Exception("Provider exploded"));

        // Act
        var result = await _blockchainService.CaptureBlockchainDataAsync(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(
            OperationResults.Common.InternalServerError,
            result.Result);

        _unitOfWork.Verify(
            x => x.AddBlockchainAsync(It.IsAny<BlockchainData>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.CompleteAsync(),
            Times.Never);
    }
    
    [Fact]
    public async Task CaptureBlockchainDataAsync_RepositorySaveFails_ReturnsInternalServerError()
    {
        var filter = GlobalUsings.CreateFilter();
        var blockchainData = GlobalUsings.CreateBlockchainData();

        SetupSuccessfulProvider(filter, blockchainData);
        SetupSuccessfulAdd(blockchainData);

        _unitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(new OperationResult<bool>(
                OperationResults.Common.InternalServerError,
                false));

        // Act
        var result = await _blockchainService.CaptureBlockchainDataAsync(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.Common.InternalServerError, result.Result);

        _unitOfWork.Verify(
            x => x.AddBlockchainAsync(blockchainData, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CompleteAsync(),
            Times.Once);
    }
    
    [Fact]
    public async Task CaptureBlockchainDataAsync_CompleteThrows_ReturnsInternalServerError()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();
        var blockchainData = GlobalUsings.CreateBlockchainData();

        SetupSuccessfulProvider(filter, blockchainData);
        SetupSuccessfulAdd(blockchainData);

        _unitOfWork
            .Setup(x => x.CompleteAsync())
            .ThrowsAsync(new Exception("Database exploded"));

        // Act
        var result = await _blockchainService.CaptureBlockchainDataAsync(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.Common.InternalServerError, result.Result);
    }
    
    [Fact]
    public async Task CaptureBlockchainDataAsync_ValidRequest_MapsProviderDataCorrectly()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();
        var blockchainData = GlobalUsings.CreateBlockchainData();
        blockchainData.Chain = filter.Chain.ToString();
        blockchainData.Coin = filter.Coin.ToString();

        SetupSuccessfulProvider(filter, blockchainData);
        SetupSuccessfulAdd(blockchainData);

        _unitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(new OperationResult<bool>(
                OperationResults.Common.Successful,
                true));

        // Act
        var result = await _blockchainService.CaptureBlockchainDataAsync(filter);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);

        Assert.Equal(blockchainData.Coin, result.Data.Coin);
        Assert.Equal(blockchainData.Chain, result.Data.Chain);

        _unitOfWork.Verify(
            x => x.AddBlockchainAsync(
                It.Is<BlockchainData>(data =>
                        data.Coin == blockchainData.Coin &&
                        data.Chain == blockchainData.Chain
                ), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CompleteAsync(),
            Times.Once);
    }
    
    #endregion

    #region GetBlockchainHistoryAsync

    [Fact]
    public async Task GetBlockchainHistoryAsync_ValidRequest_ReturnsBlockchainHistory()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();
        var blockchainData = GlobalUsings.CreateBlockchainDataList();

        var expectedResult =
            new OperationResult<IEnumerable<BlockchainData>>(
                OperationResults.Common.Successful,
                blockchainData);

        _unitOfWork
            .Setup(x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _blockchainService.GetBlockchainHistoryAsync(filter);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
        Assert.Equal(blockchainData.Count, result.Data.Count());

        _unitOfWork.Verify(
            x => x.GetBlockchainHistoryAsync(filter,It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetBlockchainHistoryAsync_NoHistory_ReturnsEmptyCollection()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();

        var expectedResult =
            new OperationResult<IEnumerable<BlockchainData>>(
                OperationResults.Common.Successful,
                Enumerable.Empty<BlockchainData>());

        _unitOfWork
            .Setup(x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _blockchainService.GetBlockchainHistoryAsync(filter);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);

        _unitOfWork.Verify(
            x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBlockchainHistoryAsync_UnitOfWorkFails_ReturnsFailure()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();

        var expectedResult =
            new OperationResult<IEnumerable<BlockchainData>>(
                OperationResults.Common.InternalServerError,
                null);

        _unitOfWork
            .Setup(x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _blockchainService.GetBlockchainHistoryAsync(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(
            OperationResults.Common.InternalServerError,
            result.Result);

        _unitOfWork.Verify(
            x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetBlockchainHistoryAsync_UnitOfWorkThrows_ReturnsInternalServerError()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();

        _unitOfWork
            .Setup(x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database exploded"));

        // Act
        var result = await _blockchainService.GetBlockchainHistoryAsync(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(
            OperationResults.Common.InternalServerError,
            result.Result);

        _unitOfWork.Verify(
            x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetBlockchainHistoryAsync_UnitOfWorkReturnsResult_ReturnsSameResult()
    {
        // Arrange
        var filter = GlobalUsings.CreateFilter();

        var expectedResult =
            new OperationResult<IEnumerable<BlockchainData>>(
                OperationResults.Common.Successful,
                new List<BlockchainData>());

        _unitOfWork
            .Setup(x => x.GetBlockchainHistoryAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _blockchainService.GetBlockchainHistoryAsync(filter);

        // Assert
        Assert.Same(expectedResult, result);
    }
    
    #endregion
    
}