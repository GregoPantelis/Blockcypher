using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Services
{
    public interface IApiClient
    {
        Task<OperationResult<TResponse>> GetAsync<TResponse>(ApiType apiType, string requestUri, CancellationToken cancellationToken = default)
            where TResponse : class;

        Task<OperationResult<TResponse>> PostAsync<TRequest, TResponse>(ApiType apiType, string requestUri, TRequest request, CancellationToken cancellationToken = default)
            where TResponse : class;
    }
}
