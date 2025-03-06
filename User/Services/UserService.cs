using EasyNetQ;
using Newtonsoft.Json.Linq;
using UserApi.Data;
using UserApi.Data.Models;
using UserApi.Services.Utils.ErrorHandling;
using UserApi.Services.Utils.TokenGenerator;

namespace UserApi.Services
{
    public class UserService
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly AppDBContext _context;
        private readonly IBus _bus;
        public UserService(TokenGenerator tokenGenerator, AppDBContext context)
        {
            _tokenGenerator = tokenGenerator;
            _context = context;
            _bus = RabbitHutch.CreateBus("host=rabbitmq");
        }

        public User GetUserById(Guid UserId)
        {
            User? User = _context.Users.FirstOrDefault(User => User.Id == UserId);
            if (User == null)
            {
                throw new ErrorException(404, "Пользователь с таким Id не найден");
            }
            return User;
        }

        public User GetUserByLogin(string Email)
        {
            User? User = _context.Users.FirstOrDefault(User => User.Email == Email);
            if (User == null)
            {
                throw new ErrorException(400, "Пользователь с такой почтой не найден.");
            }
            return User;
        }

        public List<Client> GetClients()
        {
            return _context.Users.OfType<Client>().ToList();
        }

        public void EditUser(User User)
        {
            _context.Users.Update(User);
            _context.SaveChanges();
        }

        public void RegisterUser(User User)
        {
            if (User.Role == Role.Client)
            {
                _bus.PubSub.Publish(User.Id, "CreatedClientId");
            }

            _context.Users.Add(User);
            _context.SaveChanges();
        }

        public (string AccessToken, string RefreshToken) LoginUser(User User)
        {
            var token = (AccessToken: _tokenGenerator.GenerateAccessToken(User.Id, User.Role), RefreshToken: _tokenGenerator.GenerateRefreshToken(User.Id));

            User.RefreshToken = token.RefreshToken;

            _context.Users.Update(User);
            _context.SaveChanges();

            return token;
        }

        public (string AccessToken, string RefreshToken) Refresh(User User, string LastRefreshToken)
        {
            if (User.RefreshToken != LastRefreshToken) throw new ErrorException(401, "Токен просрочен");

            var RefreshToken = _tokenGenerator.GenerateRefreshToken(User.Id);
            var AccessToken = _tokenGenerator.GenerateAccessToken(User.Id, User.Role);

            User.RefreshToken = RefreshToken;

            _context.Update(User);
            _context.SaveChanges();

            return (AccessToken, RefreshToken);
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
