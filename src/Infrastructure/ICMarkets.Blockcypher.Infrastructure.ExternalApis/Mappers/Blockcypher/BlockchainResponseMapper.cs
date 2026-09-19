using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.DataObjects.Helpers;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.Contracts.Blockcypher.Responses;

namespace ICMarkets.Blockcypher.Infrastructure.ExternalApis.Mappers.Blockcypher
{
    internal static class BlockchainResponseMapper
    {
        internal static BlockchainData MapToBlockchainData(this BlockchainResponse response, BlockchainDataFilter filter)
        {
            if (response == null)
            {
                return null;
            }

            return new BlockchainData
            {
                BChainData = response.ToJObject(),
                CreatedAt = DateTime.Now,
                UtcCreatedAt = DateTime.UtcNow,
                Chain = filter.Chain.ToString(),
                Coin = filter.Coin.ToString()
            };
        }
    }
}
