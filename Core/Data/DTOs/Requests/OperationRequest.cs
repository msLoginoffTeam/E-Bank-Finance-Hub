using Core.Data.Models;

namespace Core.Data.DTOs.Requests
{
    public class OperationRequest
    {
        public float AmountInRubles { get; set; }

        public OperationType OperationType { get; set; }
    }

    public class CashOperationRequest : OperationRequest {}

    public class CreditOperationRequest : OperationRequest
    {
        public Guid CreditId { get; set; }
    }
}
