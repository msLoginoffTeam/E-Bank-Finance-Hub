using Core.Data.Models;

namespace Core.Data.DTOs.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public float BalanceInRubles { get; set; }

        public AccountResponse(Account Account)
        {
            Id = Account.Id;
            Name = Account.Name;
            BalanceInRubles = Account.BalanceInRubles;
        }
    }
}
