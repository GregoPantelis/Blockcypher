using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Mappers;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.UoW
{
    public sealed class BlockcypherUnitOfWork : UnitOfWork<BlockcypherDbContext>, IBlockcypherUnitOfWork
    {
        private readonly ILogger<BlockcypherUnitOfWork> _logger;

        public BlockcypherUnitOfWork(BlockcypherDbContext context, ILogger<BlockcypherUnitOfWork> logger) : base(context, logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<OperationResult<bool>> AddBlockchainAsync(BlockchainData blockchainData, CancellationToken cancellationToken = default)
        {
            try
            {
                BlockchainModel model = blockchainData.MapToBlockchainModel();

                if (model is null)
                {
                    _logger.LogError("Failed to map BlockchainData to BlockchainModel.");
                    return new OperationResult<bool>(OperationResults.Common.InvalidInput, false);
                }

                return await this.Repository<BlockchainModel>().AddAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding blockchain data.");
                return new OperationResult<bool>(OperationResults.Common.InternalServerError, false); 
            }
        }

        /// <inheritdoc/>
        public async Task<OperationResult<IEnumerable<BlockchainData>>> GetBlockchainHistoryAsync(BlockchainDataFilter filter, CancellationToken cancellationToken = default)
        {
            try
            {
                string coin = filter.Coin.ToString().ToLower();
                string chain = filter.Chain.ToString().ToLower();

                List<BlockchainModel> models = await Repository<BlockchainModel>()
                    .FindBy(b => b.Coin.ToLower() == coin && b.Chain.ToLower() == chain)
                    .AsNoTracking()
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync(cancellationToken);

                if (models == null || !models.Any())
                {
                    _logger.LogWarning("No blockchain history found for Coin: {Coin}, Chain: {Chain}", coin, chain);
                    return new OperationResult<IEnumerable<BlockchainData>>(OperationResults.Common.NotFound, null);
                }

                List<BlockchainData> blockchainDataList = models.Select(b => b.MapToBlockchainData()).ToList();
                return new OperationResult<IEnumerable<BlockchainData>>(OperationResults.Common.Successful, blockchainDataList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blockchain history.");
                return new OperationResult<IEnumerable<BlockchainData>>(OperationResults.Common.InternalServerError, null);
            }
        }
    }
}
