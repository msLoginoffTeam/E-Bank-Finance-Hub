using Core.Data.Models;
using Core.Data;
using Common.ErrorHandling;
using System.Xml.Linq;

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
            if (Operation.TargetAccount.IsClosed) throw new ErrorException(403, "Целевой счет закрыт.");

            if (Operation is CashOperation)
            {
                if (Operation.OperationType == OperationType.Income)
                {
                    Operation.TargetAccount.Balance += Operation.Amount;
                }
                else
                {
                    Operation.TargetAccount.Balance -= Operation.Amount;
                    if (Operation.TargetAccount.Balance < 0) throw new ErrorException(403, "На счете не хватает денег для операции.");
                }
            }
            else if (Operation is CreditOperation)
            {
                Account BankAccount = _context.Accounts.Where(Account => Account.Client.Id == Guid.Empty && Account.Currency == Operation.TargetAccount.Currency).First();
                if (Operation.OperationType == OperationType.Income)
                {
                    BankAccount.Balance -= Operation.Amount;
                    if (BankAccount.Balance < 0) throw new ErrorException(403, "На счете банка не хватает денег для операции.");
                    Operation.TargetAccount.Balance += Operation.Amount;
                }
                else
                {
                    Operation.TargetAccount.Balance -= Operation.Amount;
                    if (Operation.TargetAccount.Balance < 0) throw new ErrorException(403, "На счете не хватает денег для операции.");
                    BankAccount.Balance += Operation.Amount;
                }
            }
            else
            {
                TransferOperation TransferOperation = (TransferOperation)Operation;

                TransferOperation.ConvertedAmount = CountConvertedAmount(TransferOperation.SenderAccount, TransferOperation.TargetAccount, Operation.Amount);

                TransferOperation.SenderAccount.Balance -= Operation.Amount;
                if (TransferOperation.SenderAccount.Balance < 0) throw new ErrorException(403, "На счете отправителя не хватает денег для операции.");
                Operation.TargetAccount.Balance += TransferOperation.ConvertedAmount;
                _context.Accounts.Update(TransferOperation.SenderAccount);
            }

            _context.Accounts.Update(Operation.TargetAccount);
            _context.Operations.Add(Operation);
            _context.SaveChanges();
        }

        public int CountConvertedAmount(Account Sender, Account Target, int SenderAmount)
        {
            int SenderCurrencyValue = _context.CurrencyCourses.First(CurrencyCourse => CurrencyCourse.Currency == Sender.Currency).Course;
            int TargetCurrencyValue = _context.CurrencyCourses.First(CurrencyCourse => CurrencyCourse.Currency == Target.Currency).Course;
            return SenderAmount * SenderCurrencyValue / TargetCurrencyValue;
        }

        public List<Operation> GetOperations(Account Account)
        {
            return _context.Operations.Where(Operation => Operation.TargetAccount == Account).OrderByDescending(Operation => Operation.Time).ToList();
        }
    }
}
