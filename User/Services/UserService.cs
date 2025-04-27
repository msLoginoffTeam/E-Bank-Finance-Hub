
using Common.ErrorHandling;
using Common.Rabbit.DTOs;
using Common.Rabbit.DTOs.Requests;
using Common.Trace;
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
		private readonly Tracer _tracer;
		public UserService(AppDBContext context, UserRabbit rabbit, Tracer tracer)
        {
            _context = context;
            _rabbit = rabbit;
            _tracer = tracer;
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
			var trace = _tracer.StartRequest(null, "UserService - SetRole", $"User: {User}, Role: {Role}");

            if (User.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager) && Role == Role.Employee)
            {
				_tracer.EndRequest(trace.DictionaryId, false, 403, "Пользователь не может быть одновременно менеджером и работником");
				throw new ErrorException(403, "Пользователь не может быть одновременно менеджером и работником");
            }
            if (User.Roles.Select(UserRole => UserRole.Role).Contains(Role))
            {
				_tracer.EndRequest(trace.DictionaryId, false, 403, "У пользователя уже есть такая роль");
				throw new ErrorException(403, "У пользователя уже есть такая роль");
            }

            User.Roles.Add(new UserRole(User, Role));

            if (Role == Role.Client)
            {
                _rabbit._bus.PubSub.Publish(new CreatedUserIdMessage() { ClientId = User.Id , TraceId = trace.TraceId}, "CreatedClientId");
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

			_tracer.EndRequest(trace.DictionaryId, true, 200, "Operated successfully");
		}

        public void RegisterUser(User User, Role Role, string Password, string TraceId)
        {
            if (Role == Role.Client)
            {
                _rabbit._bus.PubSub.Publish(new CreatedUserIdMessage() { ClientId = User.Id}, "CreatedClientId");
            }
            _rabbit._bus.PubSub.Publish(new UserAuthDTO()
            {
                Id = User.Id,
                Password = Password,
                Role = Role.ToString(),
                TraceId = TraceId
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

        public void EditRoleUser(User User, List<Role> Roles)
        {
            User.Roles = Roles.Select(Role => new UserRole(User, Role)).ToList();

            _context.Users.Update(User);
            _context.SaveChanges();
        }

        public List<string> GetEmployeeDeviceTokens()
        {
            return _context.Users.Where(User => (User.Roles.Select(UserRole => UserRole.Role).Contains(Role.Employee) || User.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager)) && User.DeviceToken != null).Select(User => User.DeviceToken!).ToList();
        }

        public string GetClientDeviceToken(Guid UserId)
        {
            return _context.Users.FirstOrDefault(User => User.Id == UserId)?.DeviceToken;
        }
    }
}
