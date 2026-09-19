using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;

namespace ICMarkets.Blockcypher.Api.Mappers;

public static class LoginContractsMapper
{
    public static LoginResponse MapToLoginResponse(this UserTokenData loginResponse)
    {
        if (loginResponse == null) return null;

        return new LoginResponse()
        {
            Token = loginResponse.Token,
            ExpiresAt = loginResponse.ExpiresAt
        };
    }
}