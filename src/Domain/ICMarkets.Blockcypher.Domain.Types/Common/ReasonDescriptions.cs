
namespace ICMarkets.Blockcypher.Domain.Types.Common
{
    public static class ReasonDescriptions
    {
        private const string DefaultDescription = "Default reason.";
        private const string UnknownDescription = "An unknown error occurred.";
        private const string InvalidInputDescription = "The input provided is invalid.";
        private const string NotFoundDescription = "The requested resource was not found.";
        private const string InternalServerErrorDescription = "An internal server error occurred.";

        private const string ApiSuccessful = "The API request was successful.";
        private const string ApiNotFound = "The requested API resource was not found.";
        private const string ApiInvalidInput = "The API request contained invalid input.";
        private const string ApiForbidden = "The API request was forbidden.";
        private const string ApiUnauthorized = "The API request was unauthorized.";
        private const string ApiBadRequest = "The API request was a bad request.";
        private const string ApiInternalServerError = "The API request resulted in an internal server error.";
        private const string ApiEmptyResponse = "The API request returned an empty response.";
        private const string ApiUnknownError = "The API request resulted in an unknown error.";
        
        private const string UserAuthSuccessful = "User Auth Successful.";
        private const string UserAuthNotFound = "User Not Found.";
        private const string UserAuthWrongPassword = "User Wrong Password.";
        private const string UserAuthInactive = "User is Inactive.";
        private const string UserAuthTypeNotSupported = "User Auth Type Not Supported.";

        /// <summary>
        /// Gets the description for a given ReasonCodes enum value.
        /// </summary>
        /// <param name="reasonCode">The ReasonCodes enum value.</param>
        /// <returns>The description string for the given ReasonCodes value.</returns>
        public static string GetDescription(ReasonCodes reasonCode)
        {
            return reasonCode switch
            {
                ReasonCodes.Default => DefaultDescription,
                ReasonCodes.Unknown => UnknownDescription,
                ReasonCodes.InvalidInput => InvalidInputDescription,
                ReasonCodes.NotFound => NotFoundDescription,
                ReasonCodes.InternalServerError => InternalServerErrorDescription,
                ReasonCodes.ApiSuccessful => ApiSuccessful,
                ReasonCodes.ApiNotFound => ApiNotFound,
                ReasonCodes.ApiInvalidInput => ApiInvalidInput,
                ReasonCodes.ApiForbidden => ApiForbidden,
                ReasonCodes.ApiUnauthorized => ApiUnauthorized,
                ReasonCodes.ApiBadRequest => ApiBadRequest,
                ReasonCodes.ApiInternalServerError => ApiInternalServerError,
                ReasonCodes.ApiEmptyResponse => ApiEmptyResponse,
                ReasonCodes.ApiUnknownError => ApiUnknownError,
                ReasonCodes.UserAuthSuccessful => UserAuthSuccessful,
                ReasonCodes.UserAuthNotFound => UserAuthNotFound,
                ReasonCodes.UserAuthWrongPassword => UserAuthWrongPassword,
                ReasonCodes.UserAuthInactive => UserAuthInactive,
                ReasonCodes.UserAuthTypeNotSupported => UserAuthTypeNotSupported,
                _ => "Unrecognized reason code."
            };
        }
    }
}
