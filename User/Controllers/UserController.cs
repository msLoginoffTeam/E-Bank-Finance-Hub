using Microsoft.AspNetCore.Mvc;
using UserApi.Data.DTOs.Requests;
using UserApi.Data.DTOs.Responses;
using UserApi.Data.Models;
using UserApi.Services;

namespace UserApi.Controllers
{
    [Route("api/users/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>  
        /// Создание клиента
        /// </summary>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> createUser(Role Role, CreateUserRequest Request)
        {
            Data.Models.User User;
            switch (Role)
            {
                case Role.Client:
                    {
                        User = new Client(Request);
                        break;
                    }
                case Role.Employee:
                    {
                        User = new Employee(Request);
                        break;
                    }
                default:
                    {
                        User = new Manager(Request);
                        break;
                    }
            }
            var token = _userService.RegisterUser(User);

            return Ok(new TokenResponse(token.AccessToken, token.RefreshToken));
        }

        /// <summary>  
        /// Создание клиента
        /// </summary>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> loginClient(LoginUserRequest Request)
        {
            Data.Models.User User = _userService.GetUserByLogin(Request.Email);
            var token = _userService.LoginUser(User);

            return Ok(new TokenResponse(token.AccessToken, token.RefreshToken));
        }
    }
}
