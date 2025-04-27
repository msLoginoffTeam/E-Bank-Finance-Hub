using Auth_Service.Data.DTOs;
using Auth_Service.Data.DTOs.Responses;
using Auth_Service.Data.Models;
using Auth_Service.Services;
using Auth_Service.Services.Utils;
using Common.Data.DTOs;
using Common.ErrorHandling;
using Common.Rabbit.DTOs;
using Common.Rabbit.DTOs.Requests;
using Common.Trace;
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
		private readonly Tracer _tracer; 

		public AuthController(AuthService authService, AuthRabbit rabbit, Tracer tracer)
		{
			_authService = authService;
			_rabbit = rabbit;
            _tracer = tracer;
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
			var trace = _tracer.StartRequest(null, "AuthController - Login");
			UserInfoResponse UserInfo = _rabbit.RpcRequest<UserInfoEmail, UserInfoResponse>(new UserInfoEmail{ Email = LoginRequest.Email, TraceId = trace.TraceId }, QueueName: "UserInfoByEmail");
            if (UserInfo.status != 200)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, UserInfo.status);
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

            if (UserInfo.IsBlocked) 
            {
				_tracer.EndRequest(trace.DictionaryId, success: false, 403, "Пользователь заблокирован");
				throw new ErrorException(403, "Пользователь заблокирован"); 
            }
            if (UserAuth.Password != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(LoginRequest.Password)))) 
            {
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Пароль не подходит");
				throw new ErrorException(400, "Пароль не подходит"); 
            }

            (string AccessToken, string RefreshToken) = _authService.Login(UserAuth, UserInfo, role);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200, returnUrl + "?accessToken=" + AccessToken + "&refreshToken=" + RefreshToken);
			return Redirect(returnUrl + "?accessToken=" + AccessToken + "&refreshToken=" + RefreshToken);
        }

		[HttpPost]
        [Authorize(Policy = "RefreshTokenAccess")]
        public ActionResult<TokenResponse> Refresh(bool IsClient)
        {
			var trace = _tracer.StartRequest(null, "AuthController - Login", $"IsClient:{IsClient}");
			var UserId = base.User.Claims.ToList().First().Value;
            UserInfoResponse UserInfo = _rabbit.RpcRequest<UserInfo, UserInfoResponse>(new UserInfo() { UserId = new Guid(UserId), TracerId = trace.TraceId }, QueueName: "UserInfoById");

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

            if (UserInfo.IsBlocked) 
            {
				_tracer.EndRequest(trace.DictionaryId, success: false, 403, "Пользователь заблокирован");
				throw new ErrorException(403, "Пользователь заблокирован"); 
            }

            (string AccessToken, string RefreshToken) = _authService.Refresh(UserAuth, UserInfo, Request.Headers.Authorization.ToString().Substring(7), role);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200, AccessToken + " " + RefreshToken);
			return new TokenResponse(AccessToken, RefreshToken);
        }

	}
}
