using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ICMarkets.Blockcypher.Api.Helpers
{
    public static class ControllerExtentions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase cbase, OperationResult<T> result)
        {
            if (!result.IsSuccessful)
            {
                if (result.Equals(OperationResults.Common.NotFound) || result.Equals(OperationResults.Api.ApiNotFound))
                {
                    return cbase.NotFound();
                }

                if (result.Equals(OperationResults.Common.InvalidInput))
                {
                    return cbase.BadRequest();
                }

                if (result.Equals(OperationResults.Common.InternalServerError))
                {
                    return cbase.StatusCode(500);
                }

                if (result.Equals(OperationResults.Common.Forbidden))
                {
                    return cbase.Forbid();
                }

                return cbase.StatusCode(500);
            }

            if (result.Data == null)
            {
                return cbase.NotFound();
            }

            return cbase.StatusCode(500);
        }
    }
}
