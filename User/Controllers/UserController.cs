using Common.ErrorHandling;
using Common.Rabbit.DTOs.Responses;
using Common.Trace;
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
        private readonly Tracer _tracer;

		public UserController(UserService userService, Tracer tracer)
		{
			_userService = userService;
			_tracer = tracer;
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
			var trace = _tracer.StartRequest(null, "UserController - createUser", $"Role:{Role} Request:{Request}");

			var EmployeeId = base.User.Claims.ToList().First().Value;

            try
            {
                if (_userService.GetUserByLogin(Request.Email) != null)
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Пользователь с такой почтой уже есть в системе.");
					return BadRequest(new ErrorResponse(400, "Пользователь с такой почтой уже есть в системе."));
                }
            }
            catch (ErrorException) { }

            User Employee = _userService.GetUserById(new Guid(EmployeeId));
            if (Role == Role.Employee && !Employee.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager))
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 403, "Создать работника может только менеджер");
				throw new ErrorException(403, "Создать работника может только менеджер");
            }
            if (Role == Role.Manager)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 403, "Нельзя создать менеджера");
				throw new ErrorException(403, "Нельзя создать менеджера");
            }

            User User = new User(Request, Role);

            _userService.RegisterUser(User, Role, Request.Password, trace.TraceId);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
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
			var trace = _tracer.StartRequest(null, "UserController - getProfile", $"ClientId:{ClientId}");

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

			_tracer.EndRequest(trace.DictionaryId, true, 200, "Operated successfully");
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
			var trace = _tracer.StartRequest(null, "UserController - setProfile", $"ClientId:{ClientId} UserDTO:{UserDTO}");

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
            if (_userService.GetUserByLogin(UserDTO.Email) != null)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "На эту почту уже зарегистрирован пользователь.");
				throw new ErrorException(400, "На эту почту уже зарегистрирован пользователь.");
            }
            User.Edit(UserDTO);

            _userService.EditUser(User);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
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
			var trace = _tracer.StartRequest(null, "UserController - getClients");
			List<User> Clients = _userService.GetClients();

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
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
			var trace = _tracer.StartRequest(null, "UserController - getEmployees");
			List<User> Employees = _userService.GetEmployees();

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
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
			var trace = _tracer.StartRequest(null, "UserController - blockUser", $"UserId:{UserId}");
			var Role = User.Claims.ToList()[2].Value;
            User BlockedUser = _userService.GetUserById(UserId);

            if (Role == "Employee")
            {
                if (BlockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager) || BlockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Employee))
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Работник может заблокировать только клиента.");
					throw new ErrorException(400, "Работник может заблокировать только клиента.");
                }
            }
            else
            {
                if (BlockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager))
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Менеджер не может заблокировать другого менеджера.");
					throw new ErrorException(400, "Менеджер не может заблокировать другого менеджера.");
                }
            }

            if (BlockedUser.IsBlocked)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Пользователь уже заблокирован.");
				throw new ErrorException(400, "Пользователь уже заблокирован.");
            }
            _userService.BlockUser(BlockedUser);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
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
			var trace = _tracer.StartRequest(null, "UserController - unblockUser", $"UserId:{UserId}");
			var Role = User.Claims.ToList()[2].Value;
            User UnblockedUser = _userService.GetUserById(UserId);

            if (Role == "Employee")
            {
                if (UnblockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager) || UnblockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Employee))
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Работник может разблокировать только клиента.");
					throw new ErrorException(400, "Работник может разблокировать только клиента.");
                }
            }
            else
            {
                if (UnblockedUser.Roles.Select(UserRole => UserRole.Role).Contains(Data.Models.Role.Manager))
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Менеджер не может разблокировать другого менеджера.");
					throw new ErrorException(400, "Менеджер не может разблокировать другого менеджера.");
                }
            }

            if (!UnblockedUser.IsBlocked)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Пользователь не заблокирован.");
				throw new ErrorException(400, "Пользователь не заблокирован.");
            }
            _userService.UnblockUser(UnblockedUser);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
        }

        /// <summary>  
        /// Получение ролей пользователя
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("get/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getRole(Guid? UserId)
		{
			var trace = _tracer.StartRequest(null, "UserController - getRole", $"UserId:{UserId}");
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

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
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
			var trace = _tracer.StartRequest(null, "UserController - editRole", $"UserId:{UserId} Roles:{Roles}");
			var role = User.Claims.ToList()[2].Value;
            User EditRoleUser = _userService.GetUserById(UserId);

            if (role == "Employee")
            {
                if (EditRoleUser.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager) || EditRoleUser.Roles.Select(UserRole => UserRole.Role).Contains(Role.Employee))
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Работник может менять роли только клиента.");
					throw new ErrorException(400, "Работник может менять роли только клиента.");
                }
            }
            else
            {
                if (EditRoleUser.Roles.Select(UserRole => UserRole.Role).Contains(Role.Manager))
				{
					_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Менеджер не может менять роли другого менеджера.");
					throw new ErrorException(400, "Менеджер не может менять роли другого менеджера.");
                }
            }
            if (Roles.Contains(Role.Manager))
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Нельзя присовить роль менеджера");
				throw new ErrorException(400, "Нельзя присовить роль менеджера");
            }

            _userService.EditRoleUser(EditRoleUser, Roles);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
        }

		/// <summary>  
		/// Соглашение на отправление уведомлений
		/// </summary>
		[Authorize]
        [HttpPost]
        [Route("set/deviceToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult setDeviceToken(string DeviceToken)
		{
			var trace = _tracer.StartRequest(null, "UserController - setDeviceToken", $"DeviceToken:{DeviceToken}");
			var UserId = base.User.Claims.First().Value;

            User User = _userService.GetUserById(new Guid(UserId));
            User.DeviceToken = DeviceToken;

            _userService.EditUser(User);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
        }
    }
}
