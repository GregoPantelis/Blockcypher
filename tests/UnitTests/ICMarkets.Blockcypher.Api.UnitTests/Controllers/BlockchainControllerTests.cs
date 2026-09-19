using FluentValidation;
using FluentValidation.Results;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Api.Controllers;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Api.UnitTests.Controllers;

public class BlockchainControllerTests
{
    private readonly Mock<ILogger<BlockchainController>> _logger;
    private readonly Mock<IBlockchainService> _blockchainService;
    private readonly Mock<IValidator<BlockchainSnaphotRequest>> _snapshotValidator;
    private readonly Mock<IValidator<BlockchainHistoryRequest>> _historyValidator;

    private readonly BlockchainController _controller;

    public BlockchainControllerTests()
    {
        _logger = new Mock<ILogger<BlockchainController>>();
        _blockchainService = new Mock<IBlockchainService>();
        _snapshotValidator = new Mock<IValidator<BlockchainSnaphotRequest>>();
        _historyValidator = new Mock<IValidator<BlockchainHistoryRequest>>();

        _controller = new BlockchainController(
            _logger.Object,
            _blockchainService.Object,
            _snapshotValidator.Object,
            _historyValidator.Object);
    }
    
    private void SetupSuccessfulHistoryValidation(BlockchainHistoryRequest request)
    {
        _historyValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task GetBlockchainData_InvalidRequest_ReturnsBadRequest()
    {
        var request = GlobalUsings.CreateSnapshotRequest();

        _snapshotValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(
            [
                new ValidationFailure(
                    nameof(BlockchainSnaphotRequest.Coin),
                    "Invalid coin.")
            ]));


        var result = await _controller.GetBlockchainData(request, CancellationToken.None);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        Assert.Equal(400, badRequest.StatusCode);

        var response = Assert.IsType<ErrorResponse>(badRequest.Value);

        Assert.Equal(OperationResults.Api.ApiInvalidInput.ToString(), response.Error);

        _blockchainService.Verify(
            x => x.CaptureBlockchainDataAsync(
                It.IsAny<BlockchainDataFilter>()),
            Times.Never);
    }

    [Fact]
    public async Task GetBlockchainData_ValidRequest_ReturnsOk()
    {
       
        var request = GlobalUsings.CreateSnapshotRequest();

        SetupSuccessfulSnapshotValidation(request);

        var blockchainData = GlobalUsings.CreateBlockchainData();

        _blockchainService
            .Setup(x => x.CaptureBlockchainDataAsync(
                It.Is<BlockchainDataFilter>(filter =>
                    filter.Coin == Coin.BTC &&
                    filter.Chain == Chain.Main)))
            .ReturnsAsync(
                new OperationResult<BlockchainData>(
                    OperationResults.Common.Successful,
                    blockchainData));

       
        var result = await _controller.GetBlockchainData(
            request,
            CancellationToken.None);

       
        var ok = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, ok.StatusCode);
        Assert.NotNull(ok.Value);

        _blockchainService.Verify(
            x => x.CaptureBlockchainDataAsync(
                It.Is<BlockchainDataFilter>(filter =>
                    filter.Coin == Coin.BTC &&
                    filter.Chain == Chain.Main)),
            Times.Once);
    }

    [Fact]
    public async Task GetBlockchainData_ServiceFails_ReturnsError()
    {
        var request = GlobalUsings.CreateSnapshotRequest();

        SetupSuccessfulSnapshotValidation(request);

        _blockchainService
            .Setup(x => x.CaptureBlockchainDataAsync(
                It.IsAny<BlockchainDataFilter>()))
            .ReturnsAsync(
                new OperationResult<BlockchainData>(
                    OperationResults.Common.InternalServerError,
                    null));

       
        var result = await _controller.GetBlockchainData(
            request,
            CancellationToken.None);
       
        var error = Assert.IsType<StatusCodeResult>(result);

        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    public async Task GetHistory_ValidRequest_ReturnsOk()
    {
       
        var request = GlobalUsings.CreateHistoryRequest();

        SetupSuccessfulHistoryValidation(request);

        var history = new List<BlockchainData>
        {
            GlobalUsings.CreateBlockchainData(),
            GlobalUsings.CreateBlockchainData()
        };

        _blockchainService
            .Setup(x => x.GetBlockchainHistoryAsync(
                It.Is<BlockchainDataFilter>(filter =>
                    filter.Coin == Coin.BTC &&
                    filter.Chain == Chain.Main)))
            .ReturnsAsync(
                new OperationResult<IEnumerable<BlockchainData>>(
                    OperationResults.Common.Successful,
                    history));

       
        var result = await _controller.GetHistory(
            request,
            CancellationToken.None);

       
        var ok = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, ok.StatusCode);
        Assert.NotNull(ok.Value);

        _blockchainService.Verify(
            x => x.GetBlockchainHistoryAsync(
                It.Is<BlockchainDataFilter>(filter =>
                    filter.Coin == Coin.BTC &&
                    filter.Chain == Chain.Main)),
            Times.Once);
    }

    [Fact]
    public async Task GetHistory_InvalidRequest_ReturnsBadRequest()
    {
        var request = GlobalUsings.CreateHistoryRequest();

        _historyValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(
            [
                new ValidationFailure(
                    nameof(BlockchainHistoryRequest.Chain),
                    "Invalid chain.")
            ]));


        var result = await _controller.GetHistory(request, CancellationToken.None);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        Assert.Equal(400, badRequest.StatusCode);

        var response = Assert.IsType<ErrorResponse>(badRequest.Value);

        Assert.Equal(OperationResults.Api.ApiInvalidInput.ToString(), response.Error);

        _blockchainService.Verify(x => x.GetBlockchainHistoryAsync(
                It.IsAny<BlockchainDataFilter>()),
            Times.Never);
    }

    private void SetupSuccessfulSnapshotValidation(
        BlockchainSnaphotRequest request)
    {
        _snapshotValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }
}