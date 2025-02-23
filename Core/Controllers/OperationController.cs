using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize]
        [HttpPost]
        [Route("{TargetAccountId}/operations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult getOperations(Guid TargetAccountId)
        {
            var ClientId = User.Claims.ToList()[0].Value;
            Account Account = _accountService.GetAccount(TargetAccountId, new Guid(ClientId));

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
        [Authorize]
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

        /// <summary>  
        /// Операции по кредиту
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("{TargetAccountId}/credit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult makeCreditOperation(Guid TargetAccountId, CreditOperationRequest Request)
        {
            var ClientId = User.Claims.ToList()[0].Value;
            Account Account = _accountService.GetAccount(TargetAccountId, new Guid(ClientId));

            CreditOperation CreditOperation = new CreditOperation(Request, Account);

            _operationService.MakeOperation(CreditOperation);

            return Ok();
        }
    }
}
