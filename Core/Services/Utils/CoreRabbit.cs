using Common.ErrorHandling;
using Common.Rabbit.DTOs.Requests;
using Core.Data.DTOs.Requests;
using Core.Data.Models;
using EasyNetQ;

namespace Core.Services.Utils
{
    public class CoreRabbit : Common.Rabbit.RabbitMQ
    {
        public CoreRabbit(IServiceProvider serviceProvider) : base(serviceProvider) { }

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
                        Operation Operation;
                        Account Account = accountService.GetAccount(Request.AccountId, Request.ClientId);

                        if (Request is CreditOperationRequest CreditRequest)
                        {
                            Operation = new CreditOperation(new OperationRequest(Request), Account, CreditRequest.CreditId);
                        }
                        else if (Request is TransferOperationRequest TransferOperationRequest)
                        {
                            Account ReceiverAccount = accountService.GetAccount(TransferOperationRequest.ReceiverAccountNumber);
                            if (ReceiverAccount == Account) return new ErrorResponse(400, "Счета получателя и отправителя совпадают");

                            Operation = new TransferOperation(Account, new OperationRequest(Request), ReceiverAccount);
                        }
                        else
                        {
                            Operation = new CashOperation(new OperationRequest(Request), Account);
                        }
                        operationService.MakeOperation(Operation);
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
        }
    }
}
