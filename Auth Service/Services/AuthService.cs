using Auth_Service.Data.Models;
using Auth_Service.Models;
using Auth_Service.Services.Utils;
using Common.Data.DTOs;
using Common.ErrorHandling;
using System.Security.Cryptography;
using System.Text;
using UserApi.Data.Models;

namespace Auth_Service.Services
{
    public class AuthService
    {
        private readonly AppDBContext _context;
        private readonly TokenGenerator _tokenGenerator;

        public AuthService(AppDBContext context, TokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public T GetUserAuthById<T>(Guid Id) where T : UserAuth
        {
            T? UserAuth = _context.Set<T>().FirstOrDefault(User => User.Id == Id);
            if (UserAuth == null) { throw new ErrorException(403, "Вы не можете сюда зайти пока у вас нет соответствующей роли"); }
            return UserAuth;

        }

        public void Register(UserAuth UserAuth)
        {
            _context.Add(UserAuth);
            _context.SaveChanges();
        }

        public (string AccessToken, string RefreshToken) Login(UserAuth UserAuth, UserInfoResponse UserInfo, string Role)
        {
            var RefreshToken = _tokenGenerator.GenerateRefreshToken(UserInfo.Id);
            var AccessToken = _tokenGenerator.GenerateAccessToken(UserInfo.Id, Role);

            if (UserAuth is ClientAuth ClientAuth)
            {
                ClientAuth.RefreshToken = RefreshToken;
                _context.Update(ClientAuth);
            }
            else if (UserAuth is EmployeeAuth EmployeeAuth)
            {
                EmployeeAuth.RefreshToken = RefreshToken;
                _context.Update(EmployeeAuth);
            }
            _context.SaveChanges();

            return (AccessToken, RefreshToken);
        }

        public (string AccessToken, string RefreshToken) Refresh(UserAuth UserAuth, UserInfoResponse UserInfo, string LastRefreshToken, string Role)
        {
            var RefreshToken = _tokenGenerator.GenerateRefreshToken(UserAuth.Id);
            var AccessToken = _tokenGenerator.GenerateAccessToken(UserAuth.Id, Role);

            if (UserAuth is ClientAuth ClientAuth)
            {
                if (ClientAuth.RefreshToken != LastRefreshToken) throw new ErrorException(401, "Токен просрочен");
                ClientAuth.RefreshToken = RefreshToken;
                _context.Update(ClientAuth);
            }
            else if (UserAuth is EmployeeAuth EmployeeAuth)
            {
                if (EmployeeAuth.RefreshToken != LastRefreshToken) throw new ErrorException(401, "Токен просрочен");
                EmployeeAuth.RefreshToken = RefreshToken;
                _context.Update(EmployeeAuth);
            }
            _context.SaveChanges();

            return (AccessToken, RefreshToken);
        }
    }
}
