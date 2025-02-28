﻿using Core.Data;
using Core.Data.Models;
using Core.Services.Utils.ErrorHandling;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class AccountService
    {
        private readonly AppDBContext _context;
        public AccountService(AppDBContext context)
        {
            _context = context;
        }

        public Client GetClient(Guid ClientId)
        {
            Client? Client = _context.Clients.FirstOrDefault(Client => Client.Id == ClientId);

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

        public List<Account> GetAccounts(Guid ClientId)
        {
            return _context.Accounts.Where(Account => Account.Client.Id == ClientId).Include(Account => Account.Client).ToList();
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

        public void DeleteAccount(Account Account)
        {
            _context.Accounts.Remove(Account);
            _context.SaveChanges();
        }
    }
}
