using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.Helpers;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ICMarkets.Blockcypher.Infrastructure.ExternalApis.ApiClients
{
    public sealed class ApiClient : IApiClient
    {
        private readonly ILogger<ApiClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<OperationResult<TResponse>> GetAsync<TResponse>(ApiType apiType, string requestUri, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            try
            {
                using (HttpClient client = _httpClientFactory.CreateClient())
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"{apiType}: API request failed with status code: {response.StatusCode}, reason: {response.ReasonPhrase}. Message: {message}");
                        return response.MapHttpResponseCodeToOperationResult<TResponse>();
                    }

                    TResponse responseContent = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);

                    if (responseContent == null)
                    {
                        _logger.LogError($"{apiType}: API request returned null content for {requestUri}");
                        return new OperationResult<TResponse>(OperationResults.Api.ApiEmptyResponse, null);
                    }

                    return new OperationResult<TResponse>(OperationResults.Api.ApiSuccessful, responseContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{apiType}: An error occurred while making a GET request to {requestUri}", ex);
                return new OperationResult<TResponse>(OperationResults.Api.ApiInternalServerError, null);
            }
        }

        public async Task<OperationResult<TResponse>> PostAsync<TRequest, TResponse>(ApiType apiType, string requestUri, TRequest request, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            try
            {
                using (HttpClient client = _httpClientFactory.CreateClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, request, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"{apiType}: API request failed with status code: {response.StatusCode}, reason: {response.ReasonPhrase}");
                        return response.MapHttpResponseCodeToOperationResult<TResponse>();
                    }

                    TResponse responseContent = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);

                    if (responseContent == null)
                    {
                        _logger.LogError($"{apiType}: API request returned null content for {requestUri}");
                        return new OperationResult<TResponse>(OperationResults.Api.ApiEmptyResponse, null);
                    }

                    return new OperationResult<TResponse>(OperationResults.Api.ApiSuccessful, responseContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{apiType}: An error occurred while making a POST request to {requestUri}", ex);
                return new OperationResult<TResponse>(OperationResults.Api.ApiInternalServerError, null);
            }
        }
    }
}
