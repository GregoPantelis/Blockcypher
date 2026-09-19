using FluentValidation;
using FluentValidation.Results;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Api.Controllers;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Api.UnitTests.Controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<IUserAuthenticationService> _authenticationService;
    private readonly Mock<IValidator<LoginRequest>> _validator;
    private readonly Mock<ILogger<AuthenticationController>> _logger;

    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _authenticationService = new Mock<IUserAuthenticationService>();
        _validator = new Mock<IValidator<LoginRequest>>();
        _logger = new Mock<ILogger<AuthenticationController>>();

        _controller = new AuthenticationController(
            _authenticationService.Object,
            _validator.Object,
            _logger.Object);
    }
    
    public void SetupSuccessfulValidation(LoginRequest request)
    {
        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Login_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = GlobalUsings.CreateLoginRequest();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                nameof(LoginRequest.Username),
                "Username is required.")
        ]);

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Login(
            request,
            CancellationToken.None);

        // Assert
        var badRequest =
            Assert.IsType<BadRequestObjectResult>(result);

        Assert.Equal(400, badRequest.StatusCode);

        var response =
            Assert.IsType<ErrorResponse>(badRequest.Value);

        Assert.Equal(
            OperationResults.Common.InvalidInput.ToString(),
            response.Error);

        _authenticationService.Verify(
            x => x.LoginAsync(
                It.IsAny<UserDataFilter>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Login_AuthenticationFails_ReturnsUnauthorized()
    {
        // Arrange
        var request = GlobalUsings.CreateLoginRequest();

        SetupSuccessfulValidation(request);

        _authenticationService
            .Setup(x => x.LoginAsync(
                It.Is<UserDataFilter>(filter =>
                    filter.Username == request.Username &&
                    filter.InputPassword == request.Password),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OperationResult<UserTokenData>(
                    OperationResults.Common.Unauthorized,
                    null));

        // Act
        var result = await _controller.Login(
            request,
            CancellationToken.None);

        // Assert
        var unauthorized =
            Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(401, unauthorized.StatusCode);

        _authenticationService.Verify(
            x => x.LoginAsync(
                It.Is<UserDataFilter>(filter =>
                    filter.Username == request.Username &&
                    filter.InputPassword == request.Password),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var request = GlobalUsings.CreateLoginRequest();

        SetupSuccessfulValidation(request);

        var tokenData = GlobalUsings.UserTokenData;

        _authenticationService
            .Setup(x => x.LoginAsync(
                It.Is<UserDataFilter>(filter =>
                    filter.Username == request.Username &&
                    filter.InputPassword == request.Password),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OperationResult<UserTokenData>(
                    OperationResults.Common.Successful,
                    tokenData));

        // Act
        var result = await _controller.Login(
            request,
            CancellationToken.None);

        // Assert
        var ok =
            Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, ok.StatusCode);

        var response =
            Assert.IsType<LoginResponse>(ok.Value);

        Assert.NotNull(response);

        _authenticationService.Verify(
            x => x.LoginAsync(
                It.Is<UserDataFilter>(filter =>
                    filter.Username == request.Username &&
                    filter.InputPassword == request.Password),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}