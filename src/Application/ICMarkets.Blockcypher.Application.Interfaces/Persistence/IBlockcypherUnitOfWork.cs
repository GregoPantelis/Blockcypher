using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Persistence
{
    public interface IBlockcypherUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Adds a new blockchain data entry to the database.
        /// </summary>
        /// <param name="blockchainData">The blockchain data to add.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>Returns an OperationResult indicating the success or failure of the operation.</returns>
        Task<OperationResult<bool>> AddBlockchainAsync(BlockchainData blockchainData, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the blockchain history based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter criteria for retrieving blockchain history.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>   
        /// <returns>Returns an OperationResult containing a collection of BlockchainData entries.</returns>
        Task<OperationResult<IEnumerable<BlockchainData>>> GetBlockchainHistoryAsync(BlockchainDataFilter filter, CancellationToken cancellationToken = default);
    }
}
