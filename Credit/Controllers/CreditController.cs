using Common.Trace;
using Core.Data.Models;
using Credit_Api.Models.requestModels;
using Credit_Api.Models.responseModels;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Models.innerModels;
using hitscord_net.Models.requestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CreditService_Patterns.Controllers;

[ApiController]
[Route("Api/Credit")]
public class CreditControllers : ControllerBase
{
    private readonly ICreditService _creditService;
	private readonly Tracer _tracer;

	public CreditControllers(ICreditService creditService, Tracer tracer)
	{
		_creditService = creditService ?? throw new ArgumentNullException(nameof(creditService));
		_tracer = tracer;
	}

	[Authorize(Roles = "Client")]
    [HttpGet]
    [Route("GetCreditsList/Client")]
    public async Task<IActionResult> GetCreditsListClient([FromQuery] ClientCreditStatusEnum? Status, [FromQuery] int ElementsNumber, [FromQuery] int PageNumber)
    {
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCreditsListClient", $"Status:{Status} ElementsNumber:{ElementsNumber} PageNumber:{PageNumber}");
		try
        {
			var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var creditsList = await _creditService.GetCreditsListClientAsync(ClientId, Status, ElementsNumber, PageNumber);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(creditsList);
        }
        catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
        }
        catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
        }
    }

	[Authorize(Roles = "Employee, Manager")]
	[HttpGet]
	[Route("GetCreditsList/Employee")]
	public async Task<IActionResult> GetCreditsListEmployee([FromQuery] Guid? ClientId, [FromQuery] ClientCreditStatusEnum? Status, [FromQuery] int ElementsNumber, [FromQuery] int PageNumber)
	{
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCreditsListEmployee", $"ClientId:{ClientId} Status:{Status} ElementsNumber:{ElementsNumber} PageNumber:{PageNumber}");
		try
		{
			var creditsList = await _creditService.GetCreditsListEmployeeAsync(ClientId, Status, ElementsNumber, PageNumber);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(creditsList);
		}
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}


	[Authorize(Roles = "Client")]
    [HttpGet]
    [Route("GetCreditHistory/Client")]
    public async Task<IActionResult> GetCreditHistoryClient([FromQuery] Guid CreditId)
    {
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCreditHistoryClient", $"CreditId:{CreditId}");
		try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var creditsList = await _creditService.GetCreditHistoryForClientAsync(ClientId, CreditId);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(creditsList);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

    [Authorize(Roles = "Employee, Manager")]
    [HttpGet]
    [Route("GetCreditHistory/Employee")]
    public async Task<IActionResult> GetCreditHistoryEmployee([FromQuery] Guid CreditId)
    {
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCreditHistoryEmployee", $"CreditId:{CreditId}");
		try
        {
            var creditsList = await _creditService.GetCreditHistoryForEmployeeAsync(CreditId);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(creditsList);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

    [Authorize(Roles = "Client, Employee, Manager")]
    [HttpGet]
    [Route("GetCreditPlans")]
    public async Task<IActionResult> GetCreditPlans([FromQuery] int ElementsNumber, [FromQuery] int PageNumber)
    {
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCreditPlans", $"ElementsNumber:{ElementsNumber} PageNumber{PageNumber}");
		try
        {
            var Role = User.Claims.ToList()[2].Value;
            var planList = await _creditService.GetCreditPlanListAsync(Role, ElementsNumber, PageNumber);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(planList);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

	[Authorize(Roles = "Employee, Manager")]
    [HttpPost]
    [Route("CreateCreditPlan")]
    public async Task<IActionResult> CreateCreditPlan([FromBody] CreateCreditPlanRequestDTO NewPlanData)
    {
		var trace = _tracer.StartRequest(null, "CreditControllers - CreateCreditPlan", $"NewPlanData:{NewPlanData}");
		try
        {
            var resultId = await _creditService.CreateCreditPlanAsync(NewPlanData);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(resultId);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

    [Authorize(Roles = "Employee, Manager")]
    [HttpPost]
    [Route("CloseCreditPlan")]
    public async Task<IActionResult> CloseCreditPlan([FromBody] CloseCreditPlanRequestDTO data)
	{
		var trace = _tracer.StartRequest(null, "CreditControllers - CloseCreditPlan", $"data:{data}");
		var Role = User.Claims.ToList()[2].Value;
        try
        {
            var resultId = await _creditService.CloseCreditPlanAsync(data.CreditPlanId);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(resultId);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

    [Authorize(Roles = "Client")]
    [HttpPost]
    [Route("GetCredit")]
    public async Task<IActionResult> GetCredit([FromBody] GetCreditRequestDTO data)
	{
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCredit", $"data:{data}");

		try
        {
            var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
            var resultId = await _creditService.GetCreditAsync(ClientId, data, trace.TraceId);
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(resultId);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

	[Authorize(Roles = "Client")]
	[HttpPost]
	[Route("PayOffTheLoan")]
	public async Task<IActionResult> PayOffTheLoan([FromBody] PayOffTheLoanRequestDTO data)
	{
		var trace = _tracer.StartRequest(null, "CreditControllers - PayOffTheLoan", $"data:{data}");

		try
		{
			var ClientId = Guid.Parse(User.Claims.ToList()[0].Value);
			var paymentResult = await _creditService.PayOffTheLoanAsync(ClientId, data, trace.TraceId);

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(paymentResult);
		}
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}


	[Authorize(Roles = "Client, Employee, Manager")]
    [HttpGet]
    [Route("GetCreditRating")]
    public async Task<IActionResult> GetCreditRating([FromQuery] Guid? ClientId)
	{
		var trace = _tracer.StartRequest(null, "CreditControllers - GetCreditRating", $"ClientId:{ClientId}");

		try
        {
            var Role = User.Claims.ToList()[2].Value;
            var UserId = Guid.Parse(User.Claims.ToList()[0].Value);
            RatingResponseDTO rating;
            switch (Role)
            {
                case "Client":
                    rating = await _creditService.GetRatingAsync(UserId, trace.TraceId);
                    break;
                case "Employee":
                    if (ClientId == null) throw new CustomException("ClientId required for this role", "CetCreditRating", "ClientId", 404);
                    rating = await _creditService.GetRatingAsync((Guid)ClientId, trace.TraceId);
                    break;
                case "Manager":
                    if (ClientId == null) throw new CustomException("ClientId required for this role", "CetCreditRating", "ClientId", 404);
                    rating = await _creditService.GetRatingAsync((Guid)ClientId, trace.TraceId);
                    break;
                default:
                    throw new CustomException("Role not found", "CetCreditRating", "Role", 404);
			}
			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(rating);
        }
		catch (CustomException ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, ex.Code, ex.Message);
			return StatusCode(ex.Code, new { Object = ex.Object, Message = ex.Message });
		}
		catch (Exception ex)
		{
			_tracer.EndRequest(trace.DictionaryId, success: false, 500, ex.Message);
			return StatusCode(500, ex.Message);
		}
	}

    /*
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
    */
}