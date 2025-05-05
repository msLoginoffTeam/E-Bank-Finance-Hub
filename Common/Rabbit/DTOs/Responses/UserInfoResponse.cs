using Common.ErrorHandling;
using Common.Rabbit.DTOs.Responses;
namespace Common.Data.DTOs
{
    public class UserInfoResponse : RabbitResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; }

        public UserInfoResponse() : base() {}
    }
}
