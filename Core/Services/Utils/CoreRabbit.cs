using Common.ErrorHandling;
using Common.Rabbit.DTOs.Requests;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core_Api.Services.Utils;
using EasyNetQ;
using Fleck;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Core.Services.Utils
{
    public class CoreRabbit : Common.Rabbit.RabbitMQ
    {
        private readonly WebSocketServerManager webSocketServerManager;
        public CoreRabbit(IServiceProvider serviceProvider, WebSocketServerManager webSocketServerManager) : base(serviceProvider)
        {
            this.webSocketServerManager = webSocketServerManager;
        }

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

            _bus.Rpc.Respond<RabbitOperationRequest, ErrorResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var operationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    try
                    {
                        object OperationResponse;

                        Operation Operation;
                        Account Account = accountService.GetAccount(Request.AccountId, Request.ClientId);

                        if (Request is CreditOperationRequest CreditRequest)
                        {
                            Operation = new CreditOperation(new OperationRequest(Request), Account, CreditRequest.CreditId, CreditRequest.Type);
                            OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                        }
                        else if (Request is TransferOperationRequest TransferOperationRequest)
                        {
                            Account ReceiverAccount = accountService.GetAccount(TransferOperationRequest.ReceiverAccountNumber);
                            if (ReceiverAccount == Account) return new ErrorResponse(400, "Счета получателя и отправителя совпадают");

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
                        operationService.MakeOperation(Operation);

                        var jsonMessage = System.Text.Json.JsonSerializer.Serialize(OperationResponse, new JsonSerializerOptions()
                        {
                            Converters = { new JsonStringEnumConverter() }
                        });

                        var Sockets = webSocketServerManager.GetBroadcast(Account.Id);
                        foreach (var Socket in Sockets != null ? Sockets : new List<IWebSocketConnection>() )
                        {
                            Socket.Send(jsonMessage);
                        }
                    }
                    catch (ErrorException ex)
                    {
                        return new ErrorResponse(ex);
                    }
                    return null;
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


            _bus.Rpc.Respond<Guid, int?>(ClientId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var client = accountService.GetClient(ClientId);
                    if (client == null) { return null; }
                    else return client.Rating;
                }
            });
        }
    }
}
