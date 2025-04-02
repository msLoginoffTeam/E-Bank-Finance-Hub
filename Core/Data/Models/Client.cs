namespace Core.Data.Models
{
    public class Client
    {
        public Guid? Id { get; set; }
        public int? Rating { get; set; }

        public Client(){}
        public Client(Guid UserId)
        {
            Id = UserId;
            Rating = 500;
        }
    }
}
