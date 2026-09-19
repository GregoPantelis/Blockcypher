using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using System.Diagnostics;

namespace ICMarkets.Blockcypher.Domain.Types.Common
{
    public static class OperationResults
    {
        public const string PREFIX = "BC-";
        public const string RESULT_NUMBERING_FORMAT = "00000";
        public const string DESCRIPTION_DELIMETER = ":";

        public static class Common
        {
            public static OperationResult Successful = new OperationResult(ReasonCodes.Successful);
            public static OperationResult InvalidInput = new OperationResult(ReasonCodes.InvalidInput);
            public static OperationResult InternalServerError = new OperationResult(ReasonCodes.InternalServerError);
            public static OperationResult NotFound = new OperationResult(ReasonCodes.NotFound);
            public static OperationResult Forbidden = new OperationResult(ReasonCodes.Forbidden);
            public static OperationResult Unauthorized = new OperationResult(ReasonCodes.Unauthorized);
        }

        public static class Api
        {
            public static OperationResult ApiSuccessful = new OperationResult(ReasonCodes.ApiSuccessful);
            public static OperationResult ApiNotFound = new OperationResult(ReasonCodes.ApiNotFound);
            public static OperationResult ApiInvalidInput = new OperationResult(ReasonCodes.ApiInvalidInput);
            public static OperationResult ApiForbidden = new OperationResult(ReasonCodes.ApiForbidden);
            public static OperationResult ApiUnauthorized = new OperationResult(ReasonCodes.ApiUnauthorized);
            public static OperationResult ApiBadRequest = new OperationResult(ReasonCodes.ApiBadRequest);
            public static OperationResult ApiInternalServerError = new OperationResult(ReasonCodes.ApiInternalServerError);
            public static OperationResult ApiEmptyResponse = new OperationResult(ReasonCodes.ApiEmptyResponse);
            public static OperationResult ApiUnknownError = new OperationResult(ReasonCodes.ApiUnknownError);
        }

        public static class UserAuth
        {
            public static OperationResult UserAuthSuccessful = new OperationResult(ReasonCodes.UserAuthSuccessful);
            public static OperationResult UserNotFound = new OperationResult(ReasonCodes.UserAuthNotFound);
            public static OperationResult UserAuthWrongPassword = new OperationResult(ReasonCodes.UserAuthWrongPassword);
            public static OperationResult UserAuthInactive = new OperationResult(ReasonCodes.UserAuthInactive);
            public static OperationResult UserAuthNotSupported = new OperationResult(ReasonCodes.UserAuthTypeNotSupported);
        }
    }
}
