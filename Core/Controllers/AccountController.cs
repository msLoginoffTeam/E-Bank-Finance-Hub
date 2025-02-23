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
        [Authorize]
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
        [Authorize]
        [HttpGet]
        [Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getAccounts()
        {
            var ClientId = User.Claims.ToList()[0].Value;

            List<Account> Accounts = _accountService.GetAccounts(new Guid(ClientId));

            return Ok(Accounts.Select(Account => new AccountResponse(Account)).ToList());
        }

        /// <summary>  
        /// Закрытие счета в банке
        /// </summary>
        [Authorize]
        [HttpDelete]
        [Route("close")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult deleteAccount(Guid AccountId)
        {
            var ClientId = User.Claims.ToList()[0].Value;
            Account Account = _accountService.GetAccount(AccountId, new Guid(ClientId));

            _accountService.DeleteAccount(Account);

            return Ok();
        }
    }
}
