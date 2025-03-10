﻿using System.Security.Cryptography;
using System.Text;
using UserApi.Data.DTOs.Requests;

namespace UserApi.Data.Models
{
    public abstract class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string? RefreshToken { get; set; }

        public bool IsBlocked { get; set; }

        public Role Role { get; set; }

        protected User() {}
        protected User(UserDTO UserDTO)
        {
            Id = Guid.NewGuid();
            Email = UserDTO.Email;
            Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(UserDTO.Password)));
            FullName = UserDTO.FullName;
            IsBlocked = false;
        }

        public void Edit(UserDTO UserDTO)
        {
            Email = UserDTO.Email;
            Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(UserDTO.Password)));
            FullName = UserDTO.FullName;
        }
    }

    public class Client : User
    {
        public Client() {}
        public Client(UserDTO Request) : base(Request) { Role = Role.Client; }
    }

    public class Employee : User
    {
        public Employee() {}
        public Employee(UserDTO Request) : base(Request) { Role = Role.Employee; }
    }

    public class Manager : Employee
    {
        public Manager() { }
        public Manager(UserDTO Request) : base(Request) { Role = Role.Manager; }
    }

    public enum Role
    {
        Client,
        Employee,
        Manager
    }
}
