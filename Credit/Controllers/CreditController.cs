using Core.Data.Models;
using Credit_Api.Models.requestModels;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Models.innerModels;
using hitscord_net.Models.requestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditService_Patterns.Controllers;

[ApiController]
[Route("Api/Credit")]
public class CreditControllers : ControllerBase
{
    private readonly ICreditService _creditService;

    public CreditControllers(ICreditService creditService)
    {
        _creditService = creditService ?? throw new ArgumentNullException(nameof(creditService));
    }

    [Authorize(Roles = "Client")]
    [HttpGet]
    [Route("GetCreditsList/Client")]
    public async Task<IActionResult> GetCreditsListClient([FromQuery] ClientCreditStatusEnum? Status, [FromQuery] int ElementsNumber, [FromQuery] int PageNumber)
    {
        try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var creditsList = await _creditService.GetCreditsListClientAsync(ClientId, Status, ElementsNumber, PageNumber);
            return Ok(creditsList);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Employee, Manager")]
    [HttpGet]
    [Route("GetCreditsList/Employee")]
    public async Task<IActionResult> GetCreditsListEmployee([FromQuery] Guid? ClientId, [FromQuery] ClientCreditStatusEnum? Status, [FromQuery] int ElementsNumber, [FromQuery] int PageNumber)
    {
        try
        {
            var creditsList = await _creditService.GetCreditsListEmployeeAsync(ClientId, Status, ElementsNumber, PageNumber);
            return Ok(creditsList);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Client")]
    [HttpGet]
    [Route("GetCreditHistory/Client")]
    public async Task<IActionResult> GetCreditHistoryClient([FromQuery] Guid CreditId)
    {
        try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var creditsList = await _creditService.GetCreditHistoryForClientAsync(ClientId, CreditId);
            return Ok(creditsList);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Employee, Manager")]
    [HttpGet]
    [Route("GetCreditHistory/Employee")]
    public async Task<IActionResult> GetCreditHistoryEmployee([FromQuery] Guid CreditId)
    {
        try
        {
            var creditsList = await _creditService.GetCreditHistoryForEmployeeAsync(CreditId);
            return Ok(creditsList);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Client, Employee, Manager")]
    [HttpGet]
    [Route("GetCreditPlans")]
    public async Task<IActionResult> GetCreditPlans([FromQuery] int ElementsNumber, [FromQuery] int PageNumber)
    {
        try
        {
            var Role = User.Claims.ToList()[2].Value;
            var planList = await _creditService.GetCreditPlanListAsync(Role, ElementsNumber, PageNumber);
            return Ok(planList);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Employee, Manager")]
    [HttpPost]
    [Route("CreateCreditPlan")]
    public async Task<IActionResult> CreateCreditPlan([FromBody] CreateCreditPlanRequestDTO NewPlanData)
    {
        try
        {
            var resultId = await _creditService.CreateCreditPlanAsync(NewPlanData);
            return Ok(resultId);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Employee, Manager")]
    [HttpPost]
    [Route("CloseCreditPlan")]
    public async Task<IActionResult> CloseCreditPlan([FromBody] CloseCreditPlanRequestDTO data)
    {
        var Role = User.Claims.ToList()[2].Value;
        try
        {
            var resultId = await _creditService.CloseCreditPlanAsync(data.CreditPlanId);
            return Ok(resultId);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Client")]
    [HttpPost]
    [Route("GetCredit")]
    public async Task<IActionResult> GetCredit([FromBody] GetCreditRequestDTO data)
    {
        try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var resultId = await _creditService.GetCreditAsync(ClientId, data);
            return Ok(resultId);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Client")]
    [HttpPost]
    [Route("PayOffTheLoan")]
    public async Task<IActionResult> PayOffTheLoan([FromBody] PayOffTheLoanRequestDTO data)
    {
        try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var paymentResult = await _creditService.PayOffTheLoanAsync(ClientId, data);
            return Ok(paymentResult);
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("check")]
    public async Task<IActionResult> check([FromQuery] Guid account)
    {
        try
        {
            return Ok(await _creditService.CheckIfHaveActiveCreditAsync(account));
        }
        catch (CustomException ex)
        {
            return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}