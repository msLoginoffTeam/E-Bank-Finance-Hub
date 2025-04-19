using Auth_Service.Data.DTOs;
using Auth_Service.Data.DTOs.Responses;
using Auth_Service.Data.Models;
using Auth_Service.Services;
using Auth_Service.Services.Utils;
using Common.Data.DTOs;
using Common.ErrorHandling;
using Common.Rabbit.DTOs.Requests;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using UserApi.Data.Models;

namespace Auth_Service.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly AuthRabbit _rabbit;

        public AuthController(AuthService authService, AuthRabbit rabbit)
        {
            _authService = authService;
            _rabbit = rabbit;
        }

        public IActionResult Error(string message)
        {
            return View(model: message);
        }

        [HttpGet]
        public ActionResult Login(bool IsClient, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.IsClient = IsClient;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginUserRequest LoginRequest, bool IsClient, string returnUrl)
        {

            UserInfoResponse UserInfo = _rabbit.RpcRequest<string, UserInfoResponse>(LoginRequest.Email, QueueName: "UserInfoByEmail");
            if (UserInfo.status != 200)
            {
                throw new ErrorException(UserInfo);
            }

            UserAuth UserAuth;
            string role;
            if (IsClient)
            {
                UserAuth = _authService.GetUserAuthById<ClientAuth>(UserInfo.Id);
                role = Role.Client.ToString();
            }
            else
            {
                UserAuth = _authService.GetUserAuthById<EmployeeAuth>(UserInfo.Id);
                role = UserInfo.Roles.Except(new List<string>() { Role.Client.ToString() }).First();
            }

            if (UserInfo.IsBlocked) { throw new ErrorException(403, "Пользователь заблокирован"); }
            if (UserAuth.Password != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(LoginRequest.Password)))) { throw new ErrorException(400, "Пароль не подходит"); }

            (string AccessToken, string RefreshToken) = _authService.Login(UserAuth, UserInfo, role);

            return Redirect(returnUrl + "?accessToken=" + AccessToken + "&refreshToken=" + RefreshToken);
        }

        [HttpPost]
        [Authorize(Policy = "RefreshTokenAccess")]
        public ActionResult<TokenResponse> Refresh(bool IsClient)
        {
            var UserId = base.User.Claims.ToList().First().Value;
            UserInfoResponse UserInfo = _rabbit.RpcRequest<Guid, UserInfoResponse>(new Guid(UserId), QueueName: "UserInfoById");

            UserAuth UserAuth;
            string role;
            if (IsClient)
            {
                UserAuth = _authService.GetUserAuthById<ClientAuth>(UserInfo.Id);
                role = Role.Client.ToString();
            }
            else
            {
                UserAuth = _authService.GetUserAuthById<EmployeeAuth>(UserInfo.Id);
                role = UserInfo.Roles.Except(new List<string>() { Role.Client.ToString() }).First();
            }

            if (UserInfo.IsBlocked) { throw new ErrorException(403, "Пользователь заблокирован"); }

            (string AccessToken, string RefreshToken) = _authService.Refresh(UserAuth, UserInfo, Request.Headers.Authorization.ToString().Substring(7), role);

            return new TokenResponse(AccessToken, RefreshToken);
        }
    }
}
