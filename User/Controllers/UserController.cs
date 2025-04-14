using Common.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using User_Api.Data.DTOs.Responses;
using UserApi.Data.DTOs.Requests;
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
        /// Создание пользователя
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employee, Manager")]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult createUser(Role Role, UserDTO Request)
        {
            var EmployeeId = base.User.Claims.ToList().First().Value;

            try
            {
                if (_userService.GetUserByLogin(Request.Email) != null) return BadRequest(new ErrorResponse(400, "Пользователь с такой почтой уже есть в системе."));
            }
            catch (ErrorException) { }

            User Employee = _userService.GetUserById(new Guid(EmployeeId));
            if (Role == Role.Employee && !Employee.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager)) throw new ErrorException(403, "Создать работника может только менеджер");
            if (Role == Role.Manager) throw new ErrorException(403, "Нельзя создать менеджера");

            User User = new User(Request, Role);

            _userService.RegisterUser(User, Role, Request.Password);

            return Ok();
        }


        /// <summary>  
        /// Получение профиля пользователя
        /// </summary>
        [Authorize(Roles = "Client, Employee, Manager")]
        [HttpGet]
        [Route("api/client/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserResponse> getProfile(Guid? ClientId)
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
        public ActionResult setProfile(Guid? ClientId, UserDTO UserDTO)
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
        [HttpGet]
        [Route("api/clients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<UserResponse>> getClients()
        {
            List<User> Clients = _userService.GetClients();

            return Ok(Clients.Select(Client => new UserResponse(Client)));
        }

        /// <summary>  
        /// Получение всех сотрудников
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet]
        [Route("api/employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<UserResponse>> getEmployees()
        {
            List<User> Employees = _userService.GetEmployees();

            return Ok(Employees.Select(Employee => new UserResponse(Employee)));
        }

        /// <summary>  
        /// Блокирование пользователя
        /// </summary>
        [Authorize(Roles = "Employee, Manager")]
        [HttpPost]
        [Route("block/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult blockUser(Guid UserId)
        {
            var Role = User.Claims.ToList()[2].Value;
            User BlockedUser = _userService.GetUserById(UserId);

            if (Role == "Employee")
            {
                if (BlockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager) || BlockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Employee)) throw new ErrorException(400, "Работник может заблокировать только клиента.");
            }
            else
            {
                if (BlockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager)) throw new ErrorException(400, "Менеджер не может заблокировать другого менеджера.");
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
        public ActionResult unblockUser(Guid UserId)
        {
            var Role = User.Claims.ToList()[2].Value;
            User UnblockedUser = _userService.GetUserById(UserId);

            if (Role == "Employee")
            {
                if (UnblockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager) || UnblockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Employee)) throw new ErrorException(400, "Работник может разблокировать только клиента.");
            }
            else
            {
                if (UnblockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager)) throw new ErrorException(400, "Менеджер не может разблокировать другого менеджера.");
            }

            if (!UnblockedUser.IsBlocked) throw new ErrorException(400, "Пользователь не заблокирован.");
            _userService.UnblockUser(UnblockedUser);

            return Ok();
        }

        /// <summary>  
        /// Получение ролей пользователя
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("get/role/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getRole(Guid? UserId)
        {
            var role = base.User.Claims.ToList()[2].Value;
            var selfUserId = base.User.Claims.ToList().First().Value;
            User User;

            if (UserId != null)
            {
                User = _userService.GetUserById((Guid)UserId);
            }
            else
            {
                User = _userService.GetUserById(new Guid(selfUserId));
            }

            return Ok(User.Roles.Select(UserRole => UserRole.Role));
        }

        /// <summary>  
        /// Изменение ролей пользователя
        /// </summary>
        [Authorize(Roles = "Employee, Manager")]
        [HttpPost]
        [Route("edit/role/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult editRole(Guid UserId, List<Role> Roles)
        {
            var role = User.Claims.ToList()[2].Value;
            User EditRoleUser = _userService.GetUserById(UserId);

            if (role == "Employee")
            {
                if (EditRoleUser.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager) || EditRoleUser.Roles.Select(UserRole => UserRole.Role).Contains(Role.Employee)) throw new ErrorException(400, "Работник может менять роли только клиента.");
            }
            else
            {
                if (EditRoleUser.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager)) throw new ErrorException(400, "Менеджер не может менять роли другого менеджера.");
            }
            if (Roles.Contains(Role.Manager)) throw new ErrorException(400, "Нельзя присовить роль менеджера");

            _userService.EditRoleUser(EditRoleUser, Roles);

            return Ok();
        }

        /// <summary>  
        /// Соглашение на отправление уведомлений
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("set/deviceToken/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult setDeviceToken(string DeviceToken)
        {
            var UserId = base.User.Claims.First().Value;

            User User = _userService.GetUserById(new Guid(UserId));
            User.DeviceToken = DeviceToken;

            _userService.EditUser(User);

            return Ok();
        }
    }
}
