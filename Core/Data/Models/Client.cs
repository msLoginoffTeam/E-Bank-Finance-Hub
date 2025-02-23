namespace Core.Data.Models
{
    public class Client
    {
        public Guid Id { get; set; }

        public Client(){}
        public Client(Guid UserId)
        {
            Id = UserId;
        }
    }
}
