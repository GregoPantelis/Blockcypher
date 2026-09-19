using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.AspNetCore.Mvc;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Mappers;
using FluentValidation;
using FluentValidation.Results;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Domain.Types.Common;

namespace ICMarkets.Blockcypher.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IUserAuthenticationService _authenticationService;
    private readonly IValidator<LoginRequest> _loginReqValidator;
    
    // GET
    public AuthenticationController(
        IUserAuthenticationService authenticationService,
        IValidator<LoginRequest> loginReqValidator,
        ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _loginReqValidator =  loginReqValidator;
        _logger = logger;
    }
    
    [HttpPost("login", Name = "login")]
    [Consumes("application/json")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _loginReqValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ErrorResponse()
            {
                Error = OperationResults.Common.InvalidInput.ToString(),
                ErrorMessage = string.Join(',', validationResult.Errors)
            });
        }
        
        OperationResult<UserTokenData> authResult = await _authenticationService.LoginAsync(new UserDataFilter()
        {
            Username = request.Username,
            InputPassword = request.Password
        }, cancellationToken);

        if (!authResult.IsSuccessful)
        {
            return Unauthorized(authResult.ToString());
        }
        
        return Ok(authResult.Data.MapToLoginResponse());
    }
}