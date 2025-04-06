using Common.Models;
using Core.Data.DTOs.Requests;

namespace Core.Data.Models
{
    public abstract class Operation
    {
        public Guid Id { get; set; }


        public int Amount { get; set; }
        public DateTime Time { get; set; }
        public OperationCategory OperationCategory { get; set; }
        public OperationType? OperationType { get; set; }


        public Account TargetAccount { get; set; }

        protected Operation(){}
        public Operation(OperationRequest Request, Account TargetAccount)
        {
            Id = Guid.NewGuid();
            Amount = Request.Amount;
            Time = DateTime.UtcNow;
            OperationType = Request.OperationType != null ? Request.OperationType : null;
            this.TargetAccount = TargetAccount;
        }
    }

    public enum OperationType
    {
        Income,
        Outcome
    }
    public enum OperationCategory
    {
        Credit,
        Cash,
        Transfer
    }

    public class CashOperation : Operation
    {
        public CashOperation() {}
        public CashOperation(OperationRequest Request, Account TargetAccount) : base(Request, TargetAccount)
        {
            OperationCategory = OperationCategory.Cash;
        }
    }

    public class CreditOperation : Operation
    {
        public Guid CreditId { get; set; }
        public CreditOperationType? Type { get; set; }
        public bool? IsSuccessful { get; set; }

        public CreditOperation() {}
        public CreditOperation(OperationRequest Request, Account TargetAccount, Guid CreditId, CreditOperationType? type, bool? isSuccessful) : base(Request, TargetAccount)
        {
            this.CreditId = CreditId;
            OperationCategory = OperationCategory.Credit;
            Type = type;
            IsSuccessful = isSuccessful;
        }
    }

    public class TransferOperation : Operation
    {
        public int ConvertedAmount { get; set; }
        public Account SenderAccount { get; set; }

        public TransferOperation() { }
        public TransferOperation(Account SenderAccount, OperationRequest Request, Account TargetAccount) : base(Request, TargetAccount)
        {
            this.SenderAccount = SenderAccount;
            OperationCategory = OperationCategory.Transfer;
        }
    }
}
