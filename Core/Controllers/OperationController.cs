using Common.ErrorHandling;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Common.Trace;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using Core.Services.Utils;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using UserApi.Data.Models;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/account/")]
    public class OperationController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly OperationService _operationService;
        private readonly CoreRabbit _rabbit;
        private readonly Tracer _tracer;

		public OperationController(AccountService accountService, OperationService operationService, CoreRabbit rabbit, Tracer tracer)
		{
			_accountService = accountService;
			_operationService = operationService;
			_rabbit = rabbit;
			_tracer = tracer;
		}

		/// <summary>  
		/// История операций
		/// </summary>
		[Authorize(Roles = "Client, Manager, Employee")]
        [HttpGet]
        [Route("{TargetAccountId}/operations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> getOperations(Guid TargetAccountId)
		{
			var trace = _tracer.StartRequest(null, "OperationController - getOperations", $"TargetAccountId:{TargetAccountId}");

			var UserId = User.Claims.ToList()[0].Value;
            var Role = User.Claims.ToList()[2].Value;

            Account Account = _accountService.GetAccount(TargetAccountId);
            if (Role == "Client" && Account.Client.Id.ToString() != UserId)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, 403, "Счет не принадлежит клиенту.");
				throw new ErrorException(403, "Счет не принадлежит клиенту.");
            }

            List<Operation> Operations = _operationService.GetOperationsByAccountId(Account.Id);

            List<object> Response = new List<object>();
            foreach (var Operation in Operations)
            {
                switch(Operation.OperationCategory)
                {
                    case OperationCategory.Credit:
                        {
                            Response.Add(new CreditOperationResponse(Operation as CreditOperation));
                            break;
                        }
                    case OperationCategory.Cash:
                        {
                            Response.Add(new CashOperationResponse(Operation as CashOperation));
                            break;
                        }
                    case OperationCategory.Transfer:
                        {
                            Response.Add(new TransferOperationResponse(Operation as TransferOperation));
                            break;
                        }
                }
            }

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok(Response);
        }

        /// <summary>  
        /// Операции с банкоматом
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("{TargetAccountId}/cash")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult makeCashOperation(Guid TargetAccountId, OperationRequest Request)
        {
			var trace = _tracer.StartRequest(null, "OperationController - makeCashOperation", $"TargetAccountId:{TargetAccountId} Request:{Request}");

			var ClientId = User.Claims.ToList()[0].Value;

            CashOperationRequest CashOperationRequest = new CashOperationRequest()
            {
                AccountId = TargetAccountId,
                ClientId = new Guid(ClientId),
                Amount = Request.Amount,
                OperationType = Request.OperationType.ToString(),
                IdempotencyKey = Guid.NewGuid(),
                TraceId = trace.TraceId
            };

            var RabbitResponse = _rabbit.RpcRequest<RabbitOperationRequest, RabbitResponse>(CashOperationRequest, QueueName: "Operations");
            if (RabbitResponse.status != 200)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, RabbitResponse.status);
				return new ObjectResult(new ErrorResponse(RabbitResponse)) { StatusCode = RabbitResponse.status }; 
            }

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
        }

        /// <summary>  
        /// Операции перевода
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("{SenderAccountId}/transfer/{ReceiverAccountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult makeTransferOperation(Guid SenderAccountId, OperationRequest Request, string ReceiverAccountNumber)
		{
			var trace = _tracer.StartRequest(null, "OperationController - makeTransferOperation", $"SenderAccountId:{SenderAccountId} Request:{Request} ReceiverAccountNumber:{ReceiverAccountNumber}");

			var ClientId = User.Claims.ToList()[0].Value;

            TransferOperationRequest TransferOperationRequest = new TransferOperationRequest()
            {
                AccountId = SenderAccountId,
                ClientId = new Guid(ClientId),
                Amount = Request.Amount,
                ReceiverAccountNumber = ReceiverAccountNumber,
                IdempotencyKey = Guid.NewGuid(),
                TraceId = trace.TraceId
            };

            var RabbitResponse = _rabbit.RpcRequest<RabbitOperationRequest, RabbitResponse>(TransferOperationRequest, QueueName: "Operations");
            if (RabbitResponse.status != 200)
			{
				_tracer.EndRequest(trace.DictionaryId, success: false, RabbitResponse.status);
				return new ObjectResult(new ErrorResponse(RabbitResponse)) { StatusCode = RabbitResponse.status }; 
            }

			_tracer.EndRequest(trace.DictionaryId, success: true, 200);
			return Ok();
        }

	}
}
