using System.Numerics;
using System.Reflection;
using Core.Data.DTOs.Requests;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Models.innerModels;
using EasyNetQ;
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
    public async Task<IActionResult> GetCreditsListClient()
    {
        try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var creditsList = await _creditService.GetCreditsListClientAsync(ClientId);
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
    public async Task<IActionResult> GetCreditsListEmployee()
    {
        try
        {
            var creditsList = await _creditService.GetCreditsListEmployeeAsync();
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
            var creditsList = await _creditService.GetCreditHistoryAsync(ClientId, CreditId);
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
    public async Task<IActionResult> GetCreditHistoryEmployee([FromQuery] Guid ClientId, [FromQuery] Guid CreditId)
    {
        try
        {
            var creditsList = await _creditService.GetCreditHistoryAsync(ClientId, CreditId);
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
    public async Task<IActionResult> GetCreditPlans()
    {
        try
        {
            var planList = await _creditService.GetCreditPlanListAsync();
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
}