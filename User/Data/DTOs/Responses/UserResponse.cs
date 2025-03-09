using UserApi.Data.Models;

namespace User_Api.Data.DTOs.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }
        
        public bool IsBlocked { get; set; }

        public UserResponse(User User)
        {
            Id = User.Id;
            Email = User.Email;
            FullName = User.FullName;
            IsBlocked = User.IsBlocked;
        }
    }
}
