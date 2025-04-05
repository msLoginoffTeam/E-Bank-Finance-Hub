
using Common.ErrorHandling;
using Common.Rabbit.DTOs;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using User_Api.Data.Models;
using UserApi.Data;
using UserApi.Data.Models;
using UserApi.Services.Utils;

namespace UserApi.Services
{
    public class UserService
    {
        private readonly AppDBContext _context;
        private readonly UserRabbit _rabbit;
        public UserService(AppDBContext context, UserRabbit rabbit)
        {
            _context = context;
            _rabbit = rabbit;
        }

        public User GetUserById(Guid UserId)
        {
            User? User = _context.Users.Include(User => User.Roles).FirstOrDefault(User => User.Id == UserId);
            if (User == null)
            {
                throw new ErrorException(404, "Пользователь с таким Id не найден");
            }
            return User;
        }

        public User? GetUserByLoginUnsafe(string Email)
        {
            User? User = _context.Users.Include(User => User.Roles).FirstOrDefault(User => User.Email == Email);
            return User;
        }

        public User GetUserByLogin(string Email)
        {
            User? User = _context.Users.Include(User => User.Roles).FirstOrDefault(User => User.Email == Email);
            if (User == null)
            {
                throw new ErrorException(400, "Пользователь с такой почтой не найден.");
            }
            return User;
        }

        public List<User> GetClients()
        {
            return _context.Users.Where(User => User.Roles.Select(UserRole => UserRole.Role).Contains(Role.Client)).ToList();
        }

        public List<User> GetEmployees()
        {
            return _context.Users.Where(User => User.Roles.Select(UserRole => UserRole.Role).Contains(Role.Employee)).ToList();
        }

        public void EditUser(User User)
        {
            _context.Users.Update(User);
            _context.SaveChanges();
        }

        public void SetRole(User User, Role Role)
        {
            if (User.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager) && Role == Role.Employee) throw new ErrorException(403, "Пользователь не может быть одновременно менеджером и работником");
            if (User.Roles.Select(UserRole => UserRole.Role).Contains(Role)) throw new ErrorException(403, "У пользователя уже есть такая роль");

            User.Roles.Add(new UserRole(User, Role));

            if (Role == Role.Client)
            {
                _rabbit._bus.PubSub.Publish(User.Id, "CreatedClientId");
                _rabbit._bus.PubSub.Publish(new UserRoleAuthDTO()
                {
                    Id = User.Id,
                    Role = Role.Client.ToString()
                });
            }
            else
            {
                _rabbit._bus.PubSub.Publish(new UserRoleAuthDTO()
                {
                    Id = User.Id,
                    Role = Role.Employee.ToString()
                });
            }

            _context.Users.Update(User);
            _context.SaveChanges();
        }

        public void RegisterUser(User User, Role Role, string Password)
        {
            if (Role == Role.Client)
            {
                _rabbit._bus.PubSub.Publish(User.Id, "CreatedClientId");
            }
            _rabbit._bus.PubSub.Publish(new UserAuthDTO()
            {
                Id = User.Id,
                Password = Password,
                Role = Role.ToString()
            });

            _context.Users.Add(User);
            _context.SaveChanges();
        }

        public void BlockUser(User User)
        {
            User.IsBlocked = true;
            _context.Users.Update(User);
            _context.SaveChanges();
        }

        public void UnblockUser(User User)
        {
            User.IsBlocked = false;
            _context.Users.Update(User);
            _context.SaveChanges();
        }
    }
}
