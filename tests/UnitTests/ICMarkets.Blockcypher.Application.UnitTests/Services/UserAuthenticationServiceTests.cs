using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Application.Services.Authentication;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Domain.Types.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Application.UnitTests.Services;

public class UserAuthenticationServiceTests
{
    private readonly Mock<ILogger<UserAuthenticationService>> _logger;
    private readonly Mock<IPasswordHasher<object>>  _hasher;
    private readonly Mock<IUserAuthUnitOfWork> _userRepository;
    private readonly Mock<IJwtTokenService> _tokenService;
    private readonly Mock<IOptions<AuthConfigOptions>> _authConfig;
    
    private UserAuthenticationService _userAuthService;
    
    public UserAuthenticationServiceTests()
    {
        _logger = new Mock<ILogger<UserAuthenticationService>>();
        _hasher = new Mock<IPasswordHasher<object>>();
        _userRepository = new Mock<IUserAuthUnitOfWork>();
        _tokenService = new Mock<IJwtTokenService>();
        _authConfig = new Mock<IOptions<AuthConfigOptions>>();
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
    }

    #region  LoginAsync
    
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // configure mocks...
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.Common.Successful, GlobalUsings.UserAuthData));

        _tokenService
            .Setup(x => x.GenerateInHouseJwtToken(It.IsAny<UserAuthData>(), It.IsAny<CancellationToken>()))
            .Returns(new OperationResult<UserTokenData>(OperationResults.Common.Successful, GlobalUsings.UserTokenData));

        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);

        _authConfig
            .Setup(x => x.Value)
            .Returns(GlobalUsings.authConfig);
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        // Act
        OperationResult<UserTokenData> result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(OperationResults.Common.Successful, result.Result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_WrongPass_ReturnsNull()
    {
        // configure mocks...
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.Common.Successful, GlobalUsings.UserAuthData));

        _tokenService
            .Setup(x => x.GenerateInHouseJwtToken(It.IsAny<UserAuthData>(), It.IsAny<CancellationToken>()))
            .Returns(new OperationResult<UserTokenData>(OperationResults.Common.Successful, GlobalUsings.UserTokenData));

        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        // Act
        var result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);

        // Assert
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.UserAuth.UserAuthWrongPassword, result.Result);
    }
    
    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_NonExistingUsername_ReturnsNull()
    {
        // configure mocks...
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.UserAuth.UserNotFound, null));

        _tokenService
            .Setup(x => x.GenerateInHouseJwtToken(It.IsAny<UserAuthData>(), It.IsAny<CancellationToken>()))
            .Returns(new OperationResult<UserTokenData>(OperationResults.Common.Successful, GlobalUsings.UserTokenData));

        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        // Act
        var result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);

        // Assert
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.UserAuth.UserNotFound, result.Result);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_InactiveUser_ReturnsNull()
    {
        // configure mocks...
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.Common.Successful, GlobalUsings.IncativeUserAuthData));
        
        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        // Act
        OperationResult<UserTokenData> result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);

        // Assert
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.UserAuth.UserAuthInactive, result.Result);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_TokenGenFails_ReturnsNull()
    {
        // configure mocks...
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.Common.Successful, GlobalUsings.UserAuthData));
        
        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        
        _tokenService
            .Setup(x => x.GenerateInHouseJwtToken(It.IsAny<UserAuthData>(), It.IsAny<CancellationToken>()))
            .Returns(new OperationResult<UserTokenData>(OperationResults.Common.InternalServerError, null));
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        // Act
        OperationResult<UserTokenData> result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);

        // Assert
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.Common.InternalServerError, result.Result);
    }
    
    [Fact]
    public async Task LoginAsync_WithValidCredentials_JwtNotConfigured_ReturnsNull()
    {
        // configure mocks...
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.Common.Successful, GlobalUsings.UserAuthData));
        
        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        
        AuthConfigOptions configOptions = GlobalUsings.authConfig;
        configOptions.TokenType = AuthToken.Undefined.ToString();
        
        _authConfig
            .Setup(x => x.Value)
            .Returns(configOptions);
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        // Act
        OperationResult<UserTokenData> result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);

        // Assert
        Assert.Null(result.Data);
        Assert.Equal(OperationResults.UserAuth.UserAuthNotSupported, result.Result);
    }

    [Fact]
    public async Task LoginAsync_IncorrectPassword_DoesNotGenerateToken()
    {
        _userRepository
            .Setup(x => x.GetUserAsync(It.IsAny<UserDataFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<UserAuthData>(OperationResults.Common.Successful, GlobalUsings.UserAuthData));
        
        _hasher
            .Setup(x => x.VerifyHashedPassword(null, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        
        _authConfig
            .Setup(x => x.Value)
            .Returns(GlobalUsings.authConfig);
        
        _userAuthService = new UserAuthenticationService(
            _logger.Object,
            _hasher.Object,
            _userRepository.Object,
            _tokenService.Object,
            _authConfig.Object);
        
        OperationResult<UserTokenData> result = await _userAuthService.LoginAsync(GlobalUsings.UserDataFilter);
        
        Assert.False(result.IsSuccessful);
        
        _tokenService
            .Verify(
                x => x.GenerateInHouseJwtToken(
                    It.IsAny<UserAuthData>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }
    
    #endregion
}