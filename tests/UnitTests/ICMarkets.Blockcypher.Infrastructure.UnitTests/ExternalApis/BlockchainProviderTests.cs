using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.ApiServices;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.Contracts.Blockcypher.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Infrastructure.UniTests.ExternalApis;

public class BlockchainProviderTests
{
    private readonly Mock<ILogger<BlockchainDataProvider>> _logger;
    private readonly Mock<IApiClient> _apiClient;

    private BlockchainDataProvider _provider;
    
    public BlockchainProviderTests()
    {
        _logger = new Mock<ILogger<BlockchainDataProvider>>();
        _apiClient = new Mock<IApiClient>();
        
        _provider = new BlockchainDataProvider(
            _logger.Object,
            GlobalUsings.CreateExternalApiOptions(),
            _apiClient.Object);
    }
    
    [Fact]
    public async Task GetBlockchainData_ValidResponse_ReturnsBlockchainData()
    {
        var filter = GlobalUsings.CreateBlockchainDataFilter();
        var apiResponse = GlobalUsings.CreateBlockchainResponse();

        _apiClient
            .Setup(x => x.GetAsync<BlockchainResponse>(
                ApiType.BlockcypherAPI,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<BlockchainResponse>(OperationResults.Common.Successful, apiResponse));

        var result = await _provider.GetBlockchainData(filter);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);

        Assert.Equal(filter.Coin.ToString(), result.Data.Coin);
        Assert.Equal(filter.Chain.ToString(), result.Data.Chain);

        _apiClient.Verify(
            x => x.GetAsync<BlockchainResponse>(
                ApiType.BlockcypherAPI,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBlockchainData_ApiReturnsError_ReturnsFailure()
    {
        var filter = GlobalUsings.CreateBlockchainDataFilter();

        _apiClient
            .Setup(x => x.GetAsync<BlockchainResponse>(
                ApiType.BlockcypherAPI,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<BlockchainResponse>(OperationResults.Common.InternalServerError, null));

        // Act
        var result = await _provider.GetBlockchainData(filter);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.Common.InternalServerError, result.Result);

        _apiClient.Verify(x => x.GetAsync<BlockchainResponse>(
                ApiType.BlockcypherAPI,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBlockchainData_NullApiResponse_ReturnsApiEmptyResponse()
    {
        var filter = GlobalUsings.CreateBlockchainDataFilter();

        _apiClient
            .Setup(x => x.GetAsync<BlockchainResponse>(
                ApiType.BlockcypherAPI,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OperationResult<BlockchainResponse>(
                    OperationResults.Common.Successful,
                    null));

        var result = await _provider.GetBlockchainData(filter);

        Assert.False(result.IsSuccessful);
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.Api.ApiEmptyResponse, result.Result);
    }

    [Fact]
    public async Task GetBlockchainData_BlockcypherConfigurationMissing_ThrowsInvalidOperationException()
    {
        var filter = GlobalUsings.CreateBlockchainDataFilter();
        var options = GlobalUsings.CreateExternalApiOptions();
        options.Value.Endpoints.First().Name = "NotBlockcypher";
        
        var provider = new BlockchainDataProvider(
            _logger.Object,
            options,
            _apiClient.Object);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetBlockchainData(filter));

        Assert.Contains("not found in configuration", exception.Message);

        _apiClient.Verify(
            x => x.GetAsync<BlockchainResponse>(
                It.IsAny<ApiType>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}