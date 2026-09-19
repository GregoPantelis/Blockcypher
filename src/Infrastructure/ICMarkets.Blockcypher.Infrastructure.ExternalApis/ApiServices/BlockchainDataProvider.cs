using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Infrastructure.Configuration.Apis;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.Contracts.Blockcypher.Responses;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.Mappers.Blockcypher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockcypher.Infrastructure.ExternalApis.ApiServices
{
    public sealed class BlockchainDataProvider : IBlockchainDataProvider
    {
        private readonly ILogger<BlockchainDataProvider> _logger;
        private readonly IOptions<ExternalApiOptions> _apiConfiguration;
        private readonly IApiClient _apiClient;
        private const string CONFIGURATION_IDENTIFIER_NAME = "Blockcypher";

        public BlockchainDataProvider(ILogger<BlockchainDataProvider> logger, IOptions<ExternalApiOptions> apiConfiguration, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
            _apiConfiguration = apiConfiguration;
        }

        public async Task<OperationResult<BlockchainData>> GetBlockchainData(BlockchainDataFilter filter)
        {
            string endpoint = GetBlockchainApiEndpoint(filter);

            OperationResult<BlockchainResponse> result = await _apiClient.GetAsync<BlockchainResponse>(ApiType.BlockcypherAPI, endpoint);

            if (!result.IsSuccessful)
            {
                _logger.LogError($"Failed to retrieve blockchain data for Coin: {filter.Coin}, Chain: {filter.Chain}. Reason: {result.ToString}");
                return new OperationResult<BlockchainData>(result.Result, null);
            }

            if (result.Data == null)
            {
                _logger.LogError($"Blockchain data for Coin: {filter.Coin}, Chain: {filter.Chain} is null.");
                return new OperationResult<BlockchainData>(OperationResults.Api.ApiEmptyResponse, null);
            }

            BlockchainData blockchainData = result.Data.MapToBlockchainData(filter);

            return new OperationResult<BlockchainData>(OperationResults.Common.Successful, blockchainData);

        }


        private string GetBlockchainApiEndpoint(BlockchainDataFilter filter)
        {
            var apiConfig = _apiConfiguration.Value.Endpoints.FirstOrDefault(e => e.Name.Equals(CONFIGURATION_IDENTIFIER_NAME, StringComparison.OrdinalIgnoreCase));
            if (apiConfig == null)
            {
                _logger.LogError("API endpoint '{ApiName}' not found in configuration.", CONFIGURATION_IDENTIFIER_NAME);
                throw new InvalidOperationException($"API endpoint '{CONFIGURATION_IDENTIFIER_NAME}' not found in configuration.");
            }

            string endpoint = $"{apiConfig.BaseUrl}/{apiConfig.ApiVersion ?? "v1"}/{filter.Coin.ToString()}/{filter.Chain.ToString()}";

            return endpoint.ToLower();
        }
    }
}