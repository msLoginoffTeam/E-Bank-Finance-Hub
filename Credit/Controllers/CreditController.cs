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
    public async Task<IActionResult> GetCreditsListClient([FromQuery] Guid ClientId)
    {
        try
        {
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

    [Authorize(Roles = "Client, Employee, Manager")]
    [HttpGet]
    [Route("GetCreditHistory")]
    public async Task<IActionResult> GetCreditHistory([FromQuery] Guid UserId, [FromQuery] Guid CreditId)
    {
        try
        {
            var creditsList = await _creditService.GetCreditHistoryAsync(CreditId);
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

    [HttpPost]
    [Route("GetCredit")]
    public async Task<IActionResult> GetCredit([FromBody] GetCreditRequestDTO data)
    {
        try
        {
            var resultId = await _creditService.GetCreditAsync(data);
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
            var paymentResult = await _creditService.PayOffTheLoanAsync(data);
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

    [HttpPost]
    [Route("PayAutomatic")]
    public async Task<IActionResult> PayAutomatic(Guid AccountId, Guid CreditId)
    {
        try
        {
            var Credit = await _creditService.GetCredit(CreditId);

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                await bus.PubSub.PublishAsync<(Guid, CreditOperationRequest)>((AccountId, new CreditOperationRequest() { CreditId = CreditId, AmountInRubles = Credit.RemainingAmount/(Credit.ClosingDate - DateTime.UtcNow).Days, OperationType = Core.Data.Models.OperationType.Outcome }));
            }
            return Ok();

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