using Auth_Service.Data.DTOs;
using Common.Rabbit.DTOs;
using Common.Rabbit.DTOs.Requests;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Cryptography;
using System.Text;

namespace Auth_Service.Data.Models
{
    public class UserAuth
    {
        public Guid Id { get; set; }
        public string Password { get; set; }

        public UserAuth() {}

        public UserAuth(UserAuthDTO UserAuthDTO)
        {
            Id = UserAuthDTO.Id;
            Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(UserAuthDTO.Password)));
        }
    }

    public class ClientAuth : UserAuth
    {
        public string? RefreshToken { get; set; }
        public ClientAuth() : base() {}
        public ClientAuth(UserAuthDTO UserAuthDTO) : base(UserAuthDTO) {}
    }

    public class EmployeeAuth : UserAuth
    {
        public string? RefreshToken { get; set; }
        public EmployeeAuth() : base() { }
        public EmployeeAuth(UserAuthDTO UserAuthDTO) : base(UserAuthDTO) { }
    }
}
