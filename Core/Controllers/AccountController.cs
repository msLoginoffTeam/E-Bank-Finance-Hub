using Common.ErrorHandling;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/accounts/")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>  
        /// Открытие счета в банке
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("open")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult createAccount(string Name)
        {
            var ClientId = User.Claims.ToList()[0].Value;
            Client Client = _accountService.GetClient(new Guid(ClientId));

            Account Account = new Account(Name, Client);
            _accountService.CreateAccount(Account);

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
            else throw new ErrorException(400, "Необходимо передать ClientId, если вы менеджер или работник.");

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
            var ClientId = User.Claims.ToList()[0].Value;
            Account Account = _accountService.GetAccount(AccountId, new Guid(ClientId));

            if (Account.IsClosed == true) throw new ErrorException(400, "Счет уже закрыт");

            _accountService.CloseAccount(Account);

            return Ok();
        }
    }
}
