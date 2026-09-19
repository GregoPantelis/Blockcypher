using ICMarkets.Blockcypher.Domain.Types.Common;

namespace ICMarkets.Blockcypher.Domain.Types.OperationalResult
{
    public class OperationResult
    {
        private string _reasonCode { get; set; }

        private string _reasonDescription { get; set; }

        public OperationResult(string reasonCode, string reasonDescription)
        {
            _reasonCode = OperationResults.PREFIX +
                reasonCode.PadLeft(OperationResults.RESULT_NUMBERING_FORMAT.Length, '0');
            _reasonDescription = reasonDescription;
        }

        public OperationResult(ReasonCodes reasonCode)
        {
            _reasonCode = OperationResults.PREFIX +
                ((int)reasonCode).ToString().PadLeft(OperationResults.RESULT_NUMBERING_FORMAT.Length, '0');
            _reasonDescription = ReasonDescriptions.GetDescription(reasonCode);
        }

        public OperationResult(string message)
        {
            string[] messageParts = message.Split(OperationResults.DESCRIPTION_DELIMETER);
            _reasonCode = messageParts[0];
            _reasonDescription = messageParts[1];
        }

        public override string ToString()
        {
            return $"ReasonCode: {_reasonCode}, ReasonDescription: {_reasonDescription}";
        }
    }
}
