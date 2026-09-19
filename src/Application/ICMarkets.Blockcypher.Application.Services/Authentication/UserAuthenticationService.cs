using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockcypher.Application.Services.Authentication;

public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly  ILogger<UserAuthenticationService> _logger;
    private readonly IPasswordHasher<object> _hasher;
    private readonly IUserAuthUnitOfWork _userAuthUnitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly AuthConfigOptions _authConfig;
    
    public UserAuthenticationService(
        ILogger<UserAuthenticationService> logger,
        IPasswordHasher<object> hasher,
        IUserAuthUnitOfWork  userAuthUnitOfWork,
        IJwtTokenService jwtTokenService,
        IOptions<AuthConfigOptions> authConfig)
    {
        _logger = logger;
        _hasher = hasher;
        _userAuthUnitOfWork = userAuthUnitOfWork;
        _jwtTokenService = jwtTokenService;
        _authConfig = authConfig.Value;
    }

    public async Task<OperationResult<UserTokenData>> LoginAsync(UserDataFilter filter, CancellationToken cancellationToken = default)
    {
        try
        {
            // get user from db
            OperationResult<UserAuthData> userResult = await _userAuthUnitOfWork.GetUserAsync(filter, cancellationToken);
            if (!userResult.IsSuccessful)
            {
                _logger.LogInformation($"User login was unsuccessful for user {filter.UserId}");
                return new OperationResult<UserTokenData>(userResult.Result, null);
            }

            if (userResult.Data == null)
            {
                _logger.LogError($"User retrieval was unsuccessful for user {filter.UserId} but user exists based on operational result");
                return new OperationResult<UserTokenData>(OperationResults.Common.InvalidInput, null);
            }
            
            if (!userResult.Data.IsActive)
                return new OperationResult<UserTokenData>(OperationResults.UserAuth.UserAuthInactive, null);
            
            // check user password
            if (!IsUserPassCorrect(userResult.Data.PasswordHash, filter.InputPassword))
            {
                return new OperationResult<UserTokenData>(OperationResults.UserAuth.UserAuthWrongPassword, null);
            }
        
            // generate token and return 
            if (_authConfig.TokenType.Equals(AuthToken.Jwt.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                OperationResult<UserTokenData> tokenDataResult = _jwtTokenService.GenerateInHouseJwtToken(userResult.Data, cancellationToken);

                if (!tokenDataResult.IsSuccessful)
                {
                    _logger.LogInformation("JWT Token generation for user login was unsuccessful");
                    return  new OperationResult<UserTokenData>(tokenDataResult.Result, null);
                }
                return  new OperationResult<UserTokenData>(tokenDataResult.Result, tokenDataResult.Data);
            }

            _logger.LogError($"Auth token type {AuthToken.Jwt} is not supported");
            return new OperationResult<UserTokenData>(OperationResults.UserAuth.UserAuthNotSupported, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an issue trying to loggin the user");
            return new OperationResult<UserTokenData>(OperationResults.Common.InternalServerError, null);
        }
    }

    private bool IsUserPassCorrect(string userPasswordHash, string userInputPass)
    { 
        PasswordVerificationResult result = _hasher.VerifyHashedPassword(
            null!,
            userPasswordHash,
            userInputPass);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}