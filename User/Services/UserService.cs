using EasyNetQ;
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
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public Data.Models.User GetUserByLogin(string Email)
        {
            Data.Models.User? User = _context.Users.FirstOrDefault(User => User.Email == Email);
            if (User == null)
            {
                throw new ErrorException(404, "Пользователь с такой почтой не найден.");
            }
            return User;
        }

        public (string AccessToken, string RefreshToken) RegisterUser(Data.Models.User User)
        {
            Guid ClientId = Guid.NewGuid();

            if (User.Role == Role.Client)
            {

            }
            _bus.PubSub.Publish(ClientId, "CreatedClientId");

            var token = (AccessToken: _tokenGenerator.GenerateAccessToken(ClientId, Role.Client), RefreshToken: _tokenGenerator.GenerateRefreshToken(ClientId));

            User.RefreshToken = token.RefreshToken;

            _context.Users.Add(User);
            _context.SaveChanges();

            return token;
        }

        public (string AccessToken, string RefreshToken) LoginUser(Data.Models.User User)
        {
            Guid ClientId = Guid.NewGuid();

            var token = (AccessToken: _tokenGenerator.GenerateAccessToken(ClientId, Role.Client), RefreshToken: _tokenGenerator.GenerateRefreshToken(ClientId));

            User.RefreshToken = token.RefreshToken;

            _context.Users.Update(User);
            _context.SaveChanges();

            return token;
        }
    }
}
