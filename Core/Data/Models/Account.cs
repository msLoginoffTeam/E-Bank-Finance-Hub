using Core.Data.DTOs.Requests;
using Core_Api.Data.DTOs.Requests;

namespace Core.Data.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string? Number { get; set; }
        public Currency Currency { get; set; }
        public int Balance { get; set; }


        public Client Client { get; set; }
        public bool IsClosed { get; set; }


        public Account() {}

        public Account(CreateAccountRequest CreateRequest, Client User)
        {
            Id = Guid.NewGuid();
            this.Client = User;
            this.Name = CreateRequest.Name;
            Random rnd = new Random();
            this.Number = new string(Enumerable.Range(0, 5).Select(x => "0123456789"[rnd.Next("0123456789".Length)]).ToArray());
            this.Currency = CreateRequest.Currency;
            this.IsClosed = false;
            this.Balance = 0;
        }
    }



    public enum Currency
    {
        Ruble,
        Dollar,
        Euro
    }
}
