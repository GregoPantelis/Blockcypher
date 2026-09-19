using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Services;

public interface IUserAuthenticationService
{
    /// <summary>
    /// Checks the authentication login request and issues session token.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="OperationResult{TData}"/> with the login data issued.</returns>
    Task<OperationResult<UserTokenData>> LoginAsync(UserDataFilter filter, CancellationToken cancellationToken = default);
}