using Common.ErrorHandling;
using Common.Rabbit;
using Common.Rabbit.DTOs.Responses;
using Core.Data;
using Core.Data.Models;
using Core.Services.Utils;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class AccountService
    {
        private readonly AppDBContext _context;
        private readonly CoreRabbit _rabbit;
        public AccountService(AppDBContext context, CoreRabbit rabbit)
        {
            _context = context;
            _rabbit = rabbit;
        }

        public Client GetClient(Guid ClientId)
        {
            Client? Client = _context.Clients.FirstOrDefault(cli => cli.Id == ClientId);

            if (Client == null)
            {
                throw new ErrorException(404, "Клиента с таким Id нет.");
            }

            return Client;
        }

        public Account GetAccount(Guid AccountId, Guid ClientId)
        {
            Account? Account = _context.Accounts.Include(Account => Account.Client).FirstOrDefault(Account => Account.Id == AccountId);

            if (Account == null)
            {
                throw new ErrorException(404, "Счета с таким Id нет.");
            }
            if (Account.Client.Id != ClientId)
            {
                throw new ErrorException(403, "Счет не принадлежит клиенту.");
            }

            return Account;
        }

        public Account GetAccount(Guid AccountId)
        {
            Account? Account = _context.Accounts.Include(Account => Account.Client).FirstOrDefault(Account => Account.Id == AccountId);

            if (Account == null)
            {
                throw new ErrorException(404, "Счета с таким Id нет.");
            }

            return Account;
        }

        public Account GetAccount(string Number)
        {
            Account? Account = _context.Accounts.Include(Account => Account.Client).FirstOrDefault(Account => Account.Number == Number);

            if (Account == null)
            {
                throw new ErrorException(404, "Счета с таким номером нет.");
            }

            return Account;
        }

        public List<Account> GetAccounts(Guid ClientId)
        {
            return _context.Accounts.Include(a => a.Client).Where(Account => Account.Client.Id == ClientId).Include(Account => Account.Client).ToList();
        }

        public void CreateClient(Guid ClientId)
        {
            _context.Clients.Add(new Client(ClientId));
            _context.SaveChanges();
        }

        public void CreateAccount(Account Account)
        {
            _context.Accounts.Add(Account);
            _context.SaveChanges();
        }

        public void CloseAccount(Account Account)
        {
            var CreditCheckResponse = _rabbit.RpcRequest<Guid, CreditCheckResponse>(Account.Id, QueueName: "CreditCheck");

            if (CreditCheckResponse.status == 404)
            {
                Account.IsClosed = true;
                _context.SaveChanges();
            }
            else throw new ErrorException(403, "На этом счету есть кредит");
        }
    }
}
