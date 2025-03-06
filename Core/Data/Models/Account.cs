using Core.Data.DTOs.Requests;

namespace Core.Data.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public Client Client { get; set; }

        public bool IsClosed { get; set; }

        public string Name { get; set; }

        public float BalanceInRubles { get; set; }

        public Account() {}

        public Account(string Name, Client User)
        {
            Id = Guid.NewGuid();
            this.Client = User;
            this.Name = Name;
            this.IsClosed = false;
            BalanceInRubles = 0;
        }
    }
}
