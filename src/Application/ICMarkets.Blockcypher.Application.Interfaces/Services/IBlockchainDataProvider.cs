using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Services
{
    public interface IBlockchainDataProvider
    {
        Task<OperationResult<BlockchainData>> GetBlockchainData(BlockchainDataFilter filter);
    }
}
