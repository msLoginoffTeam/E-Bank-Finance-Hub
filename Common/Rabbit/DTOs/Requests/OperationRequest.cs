namespace Common.Rabbit.DTOs.Requests
{
    public class RabbitOperationRequest
    {
        public Guid ClientId { get; set; }
        public Guid AccountId { get; set; }
        public int Amount { get; set; }
        public string? OperationType { get; set; }
    }
    public class CreditOperationRequest : RabbitOperationRequest
    {
        public Guid CreditId { get; set; }
    }

    public class CashOperationRequest : RabbitOperationRequest
    {

    }

    public class TransferOperationRequest : RabbitOperationRequest
    {
        public string ReceiverAccountNumber { get; set; }
    }
}
