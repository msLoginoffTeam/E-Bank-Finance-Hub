using Common.Models;
using Core.Data.DTOs.Requests;
using Core.Data.Models;

namespace Core.Data.DTOs.Responses
{
    public abstract class OperationResponse
    {
        public int Amount { get; set; }

        public DateTime Time { get; set; }

        public OperationType? OperationType { get; set; }

        public OperationCategory OperationCategory { get; set; }

        public OperationResponse(Operation Operation)
        {
            Amount = Operation.Amount;
            Time = Operation.Time;
            OperationType = Operation.OperationType != null ? Operation.OperationType : null;
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
        public CreditOperationType? Type { get; set; }
        public CreditOperationResponse(CreditOperation CreditOperation) : base(CreditOperation)
        {
            CreditId = CreditOperation.CreditId;
            Type = CreditOperation.Type;
        }
    }

    public class TransferOperationResponse : OperationResponse
    {
        public int ConvertedAmount { get; set; }
        public string TargetAccountNumber { get; set; }
        public Currency TargetAccountCurrency { get; set; }

        public TransferOperationResponse(TransferOperation TransferOperation) : base(TransferOperation)
        {
            ConvertedAmount = TransferOperation.ConvertedAmount;
            TargetAccountNumber = TransferOperation.TargetAccount.Number;
            TargetAccountCurrency = TransferOperation.TargetAccount.Currency;
        }
    }
}
