using Common.ErrorHandling;
using Common.Models;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using EasyNetQ;
using StackExchange.Redis;


namespace Core.Services.Utils
{
    public class CoreRabbit : Common.Rabbit.RabbitMQ
    {
        public CoreRabbit(IServiceProvider serviceProvider, IConnectionMultiplexer redis) : base(serviceProvider, redis) {}

        public override void Configure()
        {
            _bus.PubSub.Subscribe<Guid>("CreatedUserId_Core", ClientId =>
            {
                using (var Scope = _serviceProvider.CreateScope())
                {
                    AccountService AccountService = Scope.ServiceProvider.GetRequiredService<AccountService>();
                    AccountService.CreateClient(ClientId);
                }


            }, conf => conf.WithTopic("CreatedClientId"));

            RpcRespond(IdempotencyWrapper<RabbitOperationRequest, RabbitResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var operationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    object OperationResponse;

                    Operation Operation;
                    Account Account = accountService.GetAccount(Request.AccountId, Request.ClientId);

                    if (Request is CreditOperationRequest CreditRequest)
                    {
                        Operation = new CreditOperation(new OperationRequest(Request), Account, CreditRequest.CreditId, CreditRequest.Type, null);
                        OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                    }
                    else if (Request is TransferOperationRequest TransferOperationRequest)
                    {
                        Account ReceiverAccount = accountService.GetAccount(TransferOperationRequest.ReceiverAccountNumber);
                        if (ReceiverAccount == Account) return new RabbitResponse(400, "Счета получателя и отправителя совпадают");

                        Operation = new TransferOperation(Account, new OperationRequest(Request), ReceiverAccount);
                        ((TransferOperation)Operation).ConvertedAmount = operationService.CountConvertedAmount(Account, ReceiverAccount, Operation.Amount);
                        Console.WriteLine(((TransferOperation)Operation).ConvertedAmount);
                        OperationResponse = new TransferOperationResponse(Operation as TransferOperation);
                    }
                    else
                    {
                        Operation = new CashOperation(new OperationRequest(Request), Account);
                        OperationResponse = new CashOperationResponse(Operation as CashOperation);
                    }
                    var res = operationService.MakeOperation(Operation);

                    if (Request is CreditOperationRequest CreRequest)
                    {
                        if (res == null)
                        {
                            Operation = new CreditOperation(new OperationRequest(Request), Account, CreRequest.CreditId, CreRequest.Type, true);
                            OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                        }
                        else
                        {
                            Operation = new CreditOperation(new OperationRequest(Request), Account, CreRequest.CreditId, CreRequest.Type, false);
                            OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                        }
                    }

                    if (res != null)
                    {
                        throw new ErrorException(403, "На счете не хватает денег для операции.");
                    }
                    return new RabbitResponse(200, "");
                }
            }));

            RpcRespond<AccountExistRequest, RabbitResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var account = accountService.GetAccount(Request.AccountId, Request.ClientId);
                    if (account == null) { return new RabbitResponse(404, "Счет не найден"); }
                    else if (account.IsClosed == true) { return new RabbitResponse(403, "Счет закрыт"); }
                    else return new RabbitResponse(200, "");
                }
            });


            RpcRespond<GetRatingRequest, GetRatingResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var client = accountService.GetClient(Request.ClientId);
                    return new GetRatingResponse((int)client.Rating);
                }
            });
        }
    }
}
