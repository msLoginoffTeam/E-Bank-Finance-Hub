using Common.Models;

namespace Common.Rabbit.DTOs.Requests
{
    public class RabbitOperationRequest : RabbitRequest
    {
        public Guid ClientId { get; set; }
        public Guid AccountId { get; set; }
        public int Amount { get; set; }
        public string? OperationType { get; set; }
        public string TraceId { get; set; }

        public RabbitOperationRequest() : base() { }
    }
    public class CreditOperationRequest : RabbitOperationRequest
    {
        public Guid CreditId { get; set; }
        public CreditOperationType? Type { get; set; }

        public CreditOperationRequest() : base() { }
    }

    public class CashOperationRequest : RabbitOperationRequest
    {
        public CashOperationRequest() : base() { }
    }

    public class TransferOperationRequest : RabbitOperationRequest
    {
        public string ReceiverAccountNumber { get; set; }

        public TransferOperationRequest() : base() { }
    }
}
