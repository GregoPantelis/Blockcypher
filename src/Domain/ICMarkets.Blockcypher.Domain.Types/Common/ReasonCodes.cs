using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMarkets.Blockcypher.Domain.Types.Common
{
    public enum ReasonCodes
    {
        Default = 0,
        Successful = 1,
        InvalidInput = 2,
        InternalServerError = 3,
        NotFound = 4,
        Forbidden = 5,
        Unauthorized = 6,
        Unknown = 10,
        ApiSuccessful = 20,
        ApiNotFound = 21,
        ApiInvalidInput = 22,
        ApiForbidden = 23,
        ApiUnauthorized = 24,
        ApiBadRequest = 25,
        ApiInternalServerError = 26,
        ApiEmptyResponse = 27,
        ApiUnknownError = 28,
        UserAuthSuccessful = 30,
        UserAuthNotFound = 31,
        UserAuthWrongPassword = 32,
        UserAuthInactive = 33,
        UserAuthTypeNotSupported = 34,
    }
}
