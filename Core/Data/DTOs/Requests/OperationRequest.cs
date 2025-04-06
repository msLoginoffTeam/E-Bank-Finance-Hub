using Common.Rabbit.DTOs.Requests;
using Core.Data.Models;

namespace Core.Data.DTOs.Requests
{
    public class OperationRequest
    {
        public int Amount { get; set; }

        public OperationType? OperationType { get; set; }

        public OperationRequest() {}

        public OperationRequest(RabbitOperationRequest RabbitOperationRequest)
        {
            Amount = RabbitOperationRequest.Amount;
            OperationType = RabbitOperationRequest.OperationType != null ? (OperationType)Enum.Parse(typeof(OperationType), RabbitOperationRequest.OperationType) : null;
        }
    }
}
