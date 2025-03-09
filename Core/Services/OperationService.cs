using Core.Data.Models;
using Core.Data;
using Core.Services.Utils.ErrorHandling;

namespace Core.Services
{
    public class OperationService
    {
        private readonly AppDBContext _context;

        public OperationService(AppDBContext context)
        {
            _context = context;
        }

        public void MakeOperation(Operation Operation)
        {
            if (Operation.TargetAccount.IsClosed) throw new ErrorException(403, "Нельзя провести операцию на закрытом счете.");
            if (Operation.OperationType == OperationType.Income)
            {
                Operation.TargetAccount.BalanceInRubles += Operation.AmountInRubles;
            }
            else
            {
                Operation.TargetAccount.BalanceInRubles -= Operation.AmountInRubles;
                if (Operation.TargetAccount.BalanceInRubles < 0) throw new ErrorException(403, "На счете не хватает денег для операции.");
            }

            _context.Accounts.Update(Operation.TargetAccount);
            _context.Operations.Add(Operation);
            _context.SaveChanges();
        }

        public List<Operation> GetOperations(Account Account)
        {
            return _context.Operations.Where(Operation => Operation.TargetAccount == Account).OrderByDescending(Operation => Operation.Time).ToList();
        }
    }
}
