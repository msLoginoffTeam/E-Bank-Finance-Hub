namespace Common.Rabbit.DTOs
{
    public class UserAuthDTO
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string TraceId { get; set; }
    }
}
