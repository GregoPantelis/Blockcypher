using ICMarkets.Blockcypher.Domain.Types.Common;

namespace ICMarkets.Blockcypher.Domain.Types.OperationalResult
{
    public class OperationResult<TData>
    {
        public TData Data { get; set; }

        public bool IsSuccessful => _result == OperationResults.Common.Successful 
                                    || _result == OperationResults.Api.ApiSuccessful
                                    || _result == OperationResults.UserAuth.UserAuthSuccessful;

        public bool IsFailed => _result != OperationResults.Common.Successful
                                && _result != OperationResults.Api.ApiSuccessful
                                && _result != OperationResults.UserAuth.UserAuthSuccessful;

        private OperationResult _result { get; set; }

        public OperationResult Result => _result;

        public OperationResult(OperationResult result, TData data)
        {
            _result = result;
            Data = data;
        }

        public bool Equals(OperationResult result)
        {
            return _result == result;
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}