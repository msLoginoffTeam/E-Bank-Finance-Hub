using Common.ErrorHandling;
using Common.Models;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core_Api.Services.Utils;
using EasyNetQ;
using Fleck;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


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

            RpcIdempotent<RabbitOperationRequest, RabbitResponse>(Request =>
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
            });

            _bus.Rpc.Respond<(Guid AccountId, Guid ClientId), bool>(tuple =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var account = accountService.GetAccount(tuple.AccountId, tuple.ClientId);
                    if (account == null || account.IsClosed == true) { return false; }
                    else return true;
                }
            }, configure: x => x.WithQueueName("AccountExistCheck"));


            _bus.Rpc.Respond<GetRatingRequest, int>(ClientId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var client = accountService.GetClient(ClientId.ClientId);
                    if (client == null) { return -1; }
                    else return (int)client.Rating;
                }
            }, configure: x => x.WithQueueName("GetRating"));
        }
    }
}
