using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using User_Api.Data.DTOs.Responses;
using UserApi.Data.DTOs.Requests;
using UserApi.Data.DTOs.Responses;
using UserApi.Data.Models;
using UserApi.Services;
using UserApi.Services.Utils.ErrorHandling;

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
        /// Создание пользователя
        /// </summary>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> createUser(Role Role, UserDTO Request)
        {
            try
            {
                if (_userService.GetUserByLogin(Request.Email) != null) return BadRequest(new ErrorResponse(400, "Пользователь с такой почтой уже есть в системе."));
            }
            catch (ErrorException) {}

            User User;
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
            _userService.RegisterUser(User);

            return Ok();
        }

        /// <summary>  
        /// Вход пользователя
        /// </summary>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> loginClient(LoginUserRequest Request)
        {
            User User = _userService.GetUserByLogin(Request.Email);
            if (User.IsBlocked) { throw new ErrorException(403, "Пользователь заблокирован."); }
            if (User.Password != Request.Password) { throw new ErrorException(400, "Пароль не подходит."); }
            var token = _userService.LoginUser(User);

            return Ok(new TokenResponse(token.AccessToken, token.RefreshToken));
        }

        /// <summary>  
        /// Обновление refresh токена
        /// </summary>
        [Authorize(Policy = "RefreshTokenAccess")]
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> refresh()
        {
            var UserId = User.Claims.ToList()[0].Value;

            User user = _userService.GetUserById(new Guid(UserId));
            if (user.IsBlocked) { throw new ErrorException(403, "Пользователь заблокирован."); }
            var token = _userService.Refresh(user, Request.Headers.Authorization.ToString().Substring(7));

            return Ok(new TokenResponse(token.AccessToken, token.RefreshToken));
        }


        /// <summary>  
        /// Получение профиля пользователя
        /// </summary>
        [Authorize(Roles = "Client, Employee, Manager")]
        [HttpGet]
        [Route("api/client/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> getProfile(Guid? ClientId)
        {
            var UserId = base.User.Claims.ToList()[0].Value;
            var Role = base.User.Claims.ToList()[2].Value;

            User User;
            if (Role == "Employee" || Role == "Manager")
            {
                if (ClientId != null) User = _userService.GetUserById((Guid)ClientId);
                else User = _userService.GetUserById(new Guid(UserId));
            }
            else
            {
                User = _userService.GetUserById(new Guid(UserId));
            }

            return Ok(new UserResponse(User));
        }

        /// <summary>  
        /// Изменение профиля пользователя
        /// </summary>
        [Authorize(Roles = "Client, Employee, Manager")]
        [HttpPut]
        [Route("api/client/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> setProfile(Guid? ClientId, UserDTO UserDTO)
        {
            var UserId = base.User.Claims.ToList()[0].Value;
            var Role = base.User.Claims.ToList()[2].Value;

            User User;
            if (Role == "Employee" || Role == "Manager")
            {
                if (ClientId != null) User = _userService.GetUserById((Guid)ClientId);
                else User = _userService.GetUserById(new Guid(UserId));
            }
            else
            {
                User = _userService.GetUserById(new Guid(UserId));
            }
            if (_userService.GetUserByLogin(UserDTO.Email) != null) throw new ErrorException(400, "На эту почту уже зарегистрирован пользователь.");
            User.Edit(UserDTO);

            _userService.EditUser(User);

            return Ok(new UserResponse(User));
        }


        /// <summary>  
        /// Получение всех клиентов
        /// </summary>
        [Authorize(Roles = "Employee, Manager")]
        [HttpPost]
        [Route("api/clients/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> getClients()
        {
            List<Client> Clients = _userService.GetClients();

            return Ok(Clients.Select(Client => new UserResponse(Client)));
        }

        /// <summary>  
        /// Блокирование пользователя
        /// </summary>
        [Authorize(Roles = "Employee, Manager")]
        [HttpPost]
        [Route("block/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> blockUser(Guid UserId)
        {
            var Role = User.Claims.ToList()[2].Value;
            User BlockedUser = _userService.GetUserById(UserId);

            if (Role == "Employee")
            {
                if (BlockedUser.Role != Data.Models.Role.Client) throw new ErrorException(400, "Работник может заблокировать только пользователя.");
            }
            else
            {
                if (BlockedUser.Role == Data.Models.Role.Manager) throw new ErrorException(400, "Менеджер не может заблокировать другого менеджера.");
            }

            if (BlockedUser.IsBlocked) throw new ErrorException(400, "Пользователь уже заблокирован.");
            _userService.BlockUser(BlockedUser);

            return Ok();
        }

        /// <summary>  
        /// Разблокирование пользователя
        /// </summary>
        [Authorize(Roles = "Employee, Manager")]
        [HttpPost]
        [Route("unblock/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<TokenResponse> unblockUser(Guid UserId)
        {
            var Role = User.Claims.ToList()[2].Value;
            User UnblockedUser = _userService.GetUserById(UserId);

            if (Role == "Employee")
            {
                if (UnblockedUser.Role != Data.Models.Role.Client) throw new ErrorException(400, "Работник может разблокировать только пользователя.");
            }
            else
            {
                if (UnblockedUser.Role == Data.Models.Role.Manager) throw new ErrorException(400, "Менеджер не может разблокировать другого менеджера.");
            }

            if (!UnblockedUser.IsBlocked) throw new ErrorException(400, "Пользователь не заблокирован.");
            _userService.UnblockUser(UnblockedUser);

            return Ok();
        }
    }
}
