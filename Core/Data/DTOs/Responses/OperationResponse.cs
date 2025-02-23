using Core.Data.DTOs.Requests;
using Core.Data.Models;

namespace Core.Data.DTOs.Responses
{
    public abstract class OperationResponse
    {
        public float AmountInRubles { get; set; }

        public DateTime Time { get; set; }

        public OperationType OperationType { get; set; }

        public OperationCategory OperationCategory { get; set; }

        public OperationResponse(Operation Operation)
        {
            AmountInRubles = Operation.AmountInRubles;
            Time = Operation.Time;
            OperationType = Operation.OperationType;
            OperationCategory = Operation.OperationCategory;
        }
    }

    public class CashOperationResponse : OperationResponse
    {
        public CashOperationResponse(CashOperation CashOperation) : base(CashOperation) {}
    }

    public class CreditOperationResponse : OperationResponse
    {
        public Guid CreditId { get; set; }
        public CreditOperationResponse(CreditOperation CreditOperation) : base(CreditOperation)
        {
            CreditId = CreditOperation.CreditId;
        }
    }
}
