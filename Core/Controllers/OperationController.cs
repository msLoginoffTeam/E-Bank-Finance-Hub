using Common.ErrorHandling;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using Core.Services.Utils;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
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

        public OperationController(AccountService accountService, OperationService operationService, CoreRabbit rabbit)
        {
            _accountService = accountService;
            _operationService = operationService;
            _rabbit = rabbit;
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
            var UserId = User.Claims.ToList()[0].Value;
            var Role = User.Claims.ToList()[2].Value;

            Account Account = _accountService.GetAccount(TargetAccountId);
            if (Role == "Client" && Account.Client.Id.ToString() != UserId)
            {
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
            var ClientId = User.Claims.ToList()[0].Value;

            CashOperationRequest CashOperationRequest = new CashOperationRequest()
            {
                AccountId = TargetAccountId,
                ClientId = new Guid(ClientId),
                Amount = Request.Amount,
                OperationType = Request.OperationType.ToString(),
                IdempotencyKey = Guid.NewGuid()
            };

            var RabbitResponse = _rabbit._bus.Rpc.Request<RabbitOperationRequest, RabbitResponse>(CashOperationRequest);
            if (RabbitResponse.status != 200) { return new ObjectResult(new ErrorResponse(RabbitResponse)) { StatusCode = RabbitResponse.status }; }

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
            var ClientId = User.Claims.ToList()[0].Value;

            TransferOperationRequest TransferOperationRequest = new TransferOperationRequest()
            {
                AccountId = SenderAccountId,
                ClientId = new Guid(ClientId),
                Amount = Request.Amount,
                ReceiverAccountNumber = ReceiverAccountNumber,
                IdempotencyKey = Guid.NewGuid()
            };

            var RabbitResponse = _rabbit._bus.Rpc.Request<RabbitOperationRequest, RabbitResponse>(TransferOperationRequest);
            if (RabbitResponse.status != 200) { return new ObjectResult(new ErrorResponse(RabbitResponse)) { StatusCode = RabbitResponse.status }; }

            return Ok();
        }
    }
}
