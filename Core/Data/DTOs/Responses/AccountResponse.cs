using Core.Data.Models;

namespace Core.Data.DTOs.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }


        public string Name { get; set; }
        public string Number { get; set; }
        public float Balance { get; set; }
        public Currency Currency { get; set; }

        public bool IsClosed { get; set; }

        public AccountResponse(Account Account)
        {
            Id = Account.Id;
            Name = Account.Name;
            Number = Account.Number;
            Currency = Account.Currency;
            Balance = Account.Balance;
            IsClosed = Account.IsClosed;
        }
    }
}
