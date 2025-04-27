using System.Security.Cryptography;
using User_Api.Data.Models;
using UserApi.Data.DTOs.Requests;

namespace UserApi.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string? FullName { get; set; }

        public bool IsBlocked { get; set; }

        public string? DeviceToken { get; set; }

        public List<UserRole> Roles { get; set; }

        public User() {}
        public User(UserDTO UserDTO, Role Role)
        {
            Id = Guid.NewGuid();
            Email = UserDTO.Email;
            IsBlocked = false;
            FullName = UserDTO.FullName;
            Roles = new List<UserRole>() { new UserRole(this, Role) };
        }

        public void Edit(UserDTO UserDTO)
        {
            Email = UserDTO.Email;
            FullName = UserDTO.FullName;
        }
    }

    public enum Role
    {
        Client,
        Employee,
        Manager
    }
}
