using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UserPreferences.Services;

namespace UserPreferences.Controllers;

[Route("api/accounts/")]
[ApiController]
public class AccountsController: ControllerBase
{
    private AccountsService _accountsService { get; }

    public AccountsController(AccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult CreateHiddenAccountRecord([Required] Guid userId, [Required] Guid accountId)
    {
        return _accountsService.AddAccountRecord(userId, accountId);
    }
    
    [HttpDelete]
    [Route("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult DeleteHiddenAccountRecord([Required] Guid accountId)
    {
        return _accountsService.DeleteAccountRecord(accountId);
    }
    
    [HttpGet]
    [Route("get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> GetHiddenAccount([Required] Guid accountId)
    {
        return _accountsService.IsHiddenAccountR(accountId);
    }
}