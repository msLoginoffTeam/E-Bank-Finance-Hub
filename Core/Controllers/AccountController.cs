using Common.ErrorHandling;
using Common.Idempotency;
using Common.Rabbit.DTOs.Requests;
using Common.Trace;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using Core_Api.Data.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/accounts/")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
		private readonly Tracer _tracer;
		public AccountController(AccountService accountService, Tracer tracer)
		{
			_accountService = accountService;
			_tracer = tracer;
		}

		/// <summary>  
		/// Открытие счета в банке
		/// </summary>
		[Authorize(Roles = "Client")]
        [HttpPost]
        [Route("open")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult createAccount(CreateAccountRequest CreateRequest)
        {
			var trace = _tracer.StartRequest(null, "AccountController - createAccount", $"CreateRequest:{CreateRequest}");
			var ClientId = User.Claims.ToList()[0].Value;
			Client Client = _accountService.GetClient(new Guid(ClientId));

			Account Account = new Account(CreateRequest, Client);
			_accountService.CreateAccount(Account);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
		}

        /// <summary>  
        /// Получение счетов пользователя
        /// </summary>
        [Authorize(Roles = "Client, Manager, Employee")]
        [HttpGet]
        [Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getAccounts(Guid? ClientId)
        {
			var trace = _tracer.StartRequest(null, "AccountController - getAccounts", $"ClientId:{ClientId}");

			var UserId = User.Claims.ToList()[0].Value;
			var Role = User.Claims.ToList()[2].Value;

			List<Account> Accounts;
			if (Role == "Client")
			{
				Accounts = _accountService.GetAccounts(new Guid(UserId));
			}
			else if (ClientId != null)
			{
				Accounts = _accountService.GetAccounts((Guid)ClientId);
			}
			else
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Необходимо передать ClientId, если вы менеджер или работник.");
				throw new ErrorException(400, "Необходимо передать ClientId, если вы менеджер или работник.");
			}

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(Accounts.Select(Account => new AccountResponse(Account)).ToList());
		}

        /// <summary>  
        /// Закрытие счета в банке
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpDelete]
        [Route("close")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult closeAccount(Guid AccountId)
        {
			var trace = _tracer.StartRequest(null, "AccountController - closeAccount", $"AccountId:{AccountId}");

			var ClientId = User.Claims.ToList()[0].Value;
			Account Account = _accountService.GetAccount(AccountId, new Guid(ClientId));

			if (Account.IsClosed == true)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 400, "Счет уже закрыт");
				throw new ErrorException(400, "Счет уже закрыт");
			}

			_accountService.CloseAccount(Account, trace.TraceId);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
		}
    }
}
