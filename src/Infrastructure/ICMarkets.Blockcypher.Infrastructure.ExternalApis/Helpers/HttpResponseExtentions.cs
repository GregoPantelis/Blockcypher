using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Infrastructure.ExternalApis.Helpers
{
    internal static class HttpResponseExtentions
    {
        internal static OperationResult<TResponse> MapHttpResponseCodeToOperationResult<TResponse>(this HttpResponseMessage httpResponse)
            where TResponse : class
        {
            if (httpResponse == null) return new OperationResult<TResponse>(OperationResults.Api.ApiInternalServerError, null);

            return httpResponse.StatusCode switch
            {
                System.Net.HttpStatusCode.OK => new OperationResult<TResponse>(OperationResults.Api.ApiSuccessful, null),
                System.Net.HttpStatusCode.BadRequest => new OperationResult<TResponse>(OperationResults.Api.ApiBadRequest, null),
                System.Net.HttpStatusCode.Unauthorized => new OperationResult<TResponse>(OperationResults.Api.ApiUnauthorized, null),
                System.Net.HttpStatusCode.Forbidden => new OperationResult<TResponse>(OperationResults.Api.ApiForbidden, null),
                System.Net.HttpStatusCode.NotFound => new OperationResult<TResponse>(OperationResults.Api.ApiNotFound, null),
                (System.Net.HttpStatusCode)422 => new OperationResult<TResponse>(OperationResults.Api.ApiInvalidInput, null),
                System.Net.HttpStatusCode.InternalServerError => new OperationResult<TResponse>(OperationResults.Api.ApiInternalServerError, null),
                _ => new OperationResult<TResponse>(OperationResults.Api.ApiUnknownError, null)
            };
        }
    }
}
