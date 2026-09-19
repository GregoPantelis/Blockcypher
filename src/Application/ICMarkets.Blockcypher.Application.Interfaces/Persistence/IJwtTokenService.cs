using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Persistence;

public interface IJwtTokenService
{
    /// <summary>
    /// Generates jwt token inhouse with Blockcypher Issuer 
    /// </summary>
    /// <param name="userAuthData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    OperationResult<UserTokenData> GenerateInHouseJwtToken(UserAuthData userAuthData, CancellationToken cancellationToken = default);
}