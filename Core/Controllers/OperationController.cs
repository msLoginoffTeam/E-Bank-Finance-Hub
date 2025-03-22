using Common.ErrorHandling;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Data.Models;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/account/")]
    public class OperationController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly OperationService _operationService;
        public OperationController(AccountService accountService, OperationService operationService)
        {
            _accountService = accountService;
            _operationService = operationService;
        }

        /// <summary>  
        /// История операций
        /// </summary>
        [Authorize(Roles = "Client, Manager, Employee")]
        [HttpGet]
        [Route("{TargetAccountId}/operations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getOperations(Guid TargetAccountId)
        {
            var UserId = User.Claims.ToList()[0].Value;
            var Role = User.Claims.ToList()[2].Value;

            Account Account = _accountService.GetAccount(TargetAccountId);
            if (Role == "Client" && Account.Client.Id.ToString() != UserId)
            {
                throw new ErrorException(403, "Счет не принадлежит клиенту.");
            }

            List<Operation> Operations = _operationService.GetOperations(Account);

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
        public ActionResult makeCashOperation(Guid TargetAccountId, CashOperationRequest Request)
        {
            var ClientId = User.Claims.ToList()[0].Value;
            Account Account = _accountService.GetAccount(TargetAccountId, new Guid(ClientId));

            CashOperation CashOperation = new CashOperation(Request, Account);

            _operationService.MakeOperation(CashOperation);

            return Ok();
        }
    }
}
