using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;

namespace ICMarkets.Blockcypher.Application.Services.Blockchain
{
    public class BlockchainService : IBlockchainService
    {
        private readonly ILogger<BlockchainService> _logger;
        private readonly IBlockchainDataProvider _provider;
        private readonly IBlockcypherUnitOfWork _unitOfWork;

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainDataProvider provider, IBlockcypherUnitOfWork unitOfWork)
        {
            _logger = logger;
            _provider = provider;
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult<BlockchainData>> CaptureBlockchainDataAsync(BlockchainDataFilter filter)
        {
            try
            {
                OperationResult<BlockchainData> blockchainResult = await _provider.GetBlockchainData(filter);

                if (!blockchainResult.IsSuccessful)
                {
                    _logger.LogError($"Failed to retrieve blockchain data for Coin: {filter.Coin}, Chain: {filter.Chain}. Reason: {blockchainResult.ToString}");
                    return new OperationResult<BlockchainData>(blockchainResult.Result, null);
                }

                if (blockchainResult.Data == null)
                {
                    _logger.LogWarning($"No blockchain data found for Coin: {filter.Coin}, Chain: {filter.Chain}.");
                    return new OperationResult<BlockchainData>(OperationResults.Common.NotFound, null);
                }

                OperationResult<bool> addResult = await _unitOfWork.AddBlockchainAsync(blockchainResult.Data);

                if (!addResult.IsSuccessful)
                {
                    _logger.LogError($"Failed to add blockchain data to the database for Coin: {filter.Coin}, Chain: {filter.Chain}. Reason: {addResult.ToString}");
                    return new OperationResult<BlockchainData>(addResult.Result, null);
                }

                OperationResult<bool> saveResult = await _unitOfWork.CompleteAsync();

                if (!saveResult.IsSuccessful)
                {
                    _logger.LogError($"Failed to save blockchain data to the database for Coin: {filter.Coin}, Chain: {filter.Chain}. Reason: {saveResult.ToString}");
                    return new OperationResult<BlockchainData>(saveResult.Result, null);
                }

                return saveResult.IsSuccessful 
                    ? new OperationResult<BlockchainData>(saveResult.Result, blockchainResult.Data)
                    : new OperationResult<BlockchainData>(saveResult.Result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting blockchain data.");
                return new OperationResult<BlockchainData>(OperationResults.Common.InternalServerError, null);
            }
            
        }

        public async Task<OperationResult<IEnumerable<BlockchainData>>> GetBlockchainHistoryAsync(BlockchainDataFilter filter)
        {
            try
            {
                OperationResult<IEnumerable<BlockchainData>> result = await _unitOfWork.GetBlockchainHistoryAsync(filter);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting blockchain history.");
                return new OperationResult<IEnumerable<BlockchainData>>(OperationResults.Common.InternalServerError, null);
            }
        }
    }
}
