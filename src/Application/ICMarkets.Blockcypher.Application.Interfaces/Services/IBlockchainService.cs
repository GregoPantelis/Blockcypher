using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Services
{
    public interface IBlockchainService
    {
        /// <summary>
        /// Captures the blockcypher API data based on the provided filter and returns the result.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>An <see cref="OperationResult{BlockchainData}"/> representing the result of the capture operation.</returns> 
        Task<OperationResult<BlockchainData>> CaptureBlockchainDataAsync(BlockchainDataFilter filter);

        /// <summary>
        /// Gets the blockchain history based on the provided filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>An <see cref="OperationResult{BlockchainData}"/> representing the result of the get operation.</returns>
        Task<OperationResult<IEnumerable<BlockchainData>>> GetBlockchainHistoryAsync(BlockchainDataFilter filter);
    }
}
