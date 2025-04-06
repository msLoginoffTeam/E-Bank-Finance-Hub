using Core.Data.Models;
using Core.Data;
using Common.ErrorHandling;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class OperationService
    {
        private readonly AppDBContext _context;

        public OperationService(AppDBContext context)
        {
            _context = context;
        }

        public bool? MakeOperation(Operation Operation)
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
                _context.Operations.Add(Operation);
            }
            else if (Operation is CreditOperation creditOperation)
            {
                Account BankAccount = _context.Accounts.Where(Account => Account.Client.Id == Guid.Empty && Account.Currency == creditOperation.TargetAccount.Currency).First();
                Client UserCli = _context.Clients.Where(c => c.Id == creditOperation.TargetAccount.Client.Id).First();
                if (creditOperation.OperationType == OperationType.Income)
                {
                    BankAccount.Balance -= creditOperation.Amount;
                    if (BankAccount.Balance < 0) throw new ErrorException(403, "На счете банка не хватает денег для операции.");
                    creditOperation.TargetAccount.Balance += creditOperation.Amount;
                }
                else
                {
                    var sum = creditOperation.TargetAccount.Balance - creditOperation.Amount;
                    if (sum < 0)
                    {
                        UserCli.Rating = UserCli.Rating > 0 ? --UserCli.Rating : UserCli.Rating;
                        creditOperation.IsSuccessful = false;
                        _context.Operations.Add(Operation);
                        _context.Clients.Update(UserCli);
                        _context.SaveChanges();
                        return false;
                    }
                    UserCli.Rating = UserCli.Rating < 1000 ? ++UserCli.Rating : UserCli.Rating;
                    BankAccount.Balance += Operation.Amount;
                    creditOperation.IsSuccessful = true;
                    creditOperation.TargetAccount.Balance = sum;
                }
                _context.Operations.Add(Operation);
                _context.Clients.Update(UserCli);
            }
            else
            {
                TransferOperation TransferOperation = (TransferOperation)Operation;
                TransferOperation.SenderAccount.Balance -= Operation.Amount;
                if (TransferOperation.SenderAccount.Balance < 0) throw new ErrorException(403, "На счете отправителя не хватает денег для операции.");
                Operation.TargetAccount.Balance += TransferOperation.ConvertedAmount;
                _context.Accounts.Update(TransferOperation.SenderAccount);
                _context.Operations.Add(TransferOperation);
            }

            _context.Accounts.Update(Operation.TargetAccount);
            _context.SaveChanges();


            return null;
        }

        public int CountConvertedAmount(Account Sender, Account Target, int SenderAmount)
        {
            int SenderCurrencyValue = _context.CurrencyCourses.First(CurrencyCourse => CurrencyCourse.Currency == Sender.Currency).Course;
            int TargetCurrencyValue = _context.CurrencyCourses.First(CurrencyCourse => CurrencyCourse.Currency == Target.Currency).Course;
            Console.WriteLine(SenderCurrencyValue);
            Console.WriteLine(TargetCurrencyValue);
            return SenderAmount * SenderCurrencyValue / TargetCurrencyValue;
        }

        public List<Operation> GetOperationsByAccountId(Guid AccountId)
        {
            var Operations = _context.Operations
                .Where(Operation => Operation.TargetAccount.Id == AccountId);

            var TransferOperations = _context.Operations
                .Where(Operation => (Operation as TransferOperation).SenderAccount.Id == AccountId);


            return Operations.Concat(TransferOperations).Include(Operation => Operation.TargetAccount)
                .OrderByDescending(Operation => Operation.Time)  
                .ToList();
        }
    }
}
