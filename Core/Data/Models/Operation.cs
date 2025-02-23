using Core.Data.DTOs.Requests;

namespace Core.Data.Models
{
    public abstract class Operation
    {
        public Guid Id { get; set; }

        public float AmountInRubles { get; set; }

        public DateTime Time { get; set; }

        public OperationType OperationType { get; set; }

        public OperationCategory OperationCategory { get; set; }

        public Account TargetAccount { get; set; }

        protected Operation(){}

        public Operation(OperationRequest Request, Account TargetAccount)
        {
            Id = Guid.NewGuid();
            AmountInRubles = Request.AmountInRubles;
            Time = DateTime.UtcNow;
            OperationType = Request.OperationType;
            this.TargetAccount = TargetAccount;
        }
    }

    public class CashOperation : Operation
    {
        public CashOperation() {}
        public CashOperation(CashOperationRequest Request, Account TargetAccount) : base(Request, TargetAccount) { OperationCategory = OperationCategory.Cash; }
    }

    public class CreditOperation : Operation
    {
        public Guid CreditId { get; set; }

        public CreditOperation() {}
        public CreditOperation(CreditOperationRequest Request, Account TargetAccount) : base(Request, TargetAccount)
        {
            CreditId = Request.CreditId;
            OperationCategory = OperationCategory.Credit;
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
        Cash
    }
}
