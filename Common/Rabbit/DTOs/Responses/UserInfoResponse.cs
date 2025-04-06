using Common.ErrorHandling;
namespace Common.Data.DTOs
{
    public class UserInfoResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; }

        public string error { get; set; }

        public UserInfoResponse() {}
    }
}
