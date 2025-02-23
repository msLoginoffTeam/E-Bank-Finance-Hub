using UserApi.Data.Models;

namespace UserApi.Data.DTOs.Requests
{
    public class CreateUserRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }
    }
}
