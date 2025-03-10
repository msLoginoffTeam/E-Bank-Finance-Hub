using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services.Utils.ErrorHandling;
using CoreApi.Models.innerModels;
using EasyNetQ;
using UserApi.Data.Models;

namespace Core.Services.Utils
{
    public class RabbitMQ
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQ(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _bus = RabbitHutch.CreateBus("host=rabbitmq");

            _bus.PubSub.Subscribe<Guid>("CreatedUserId_Core", ClientId =>
            {
                using (var Scope = _serviceProvider.CreateScope())
                {
                    AccountService AccountService = Scope.ServiceProvider.GetRequiredService<AccountService>();
                    AccountService.CreateClient(ClientId);
                }


            }, conf => conf.WithTopic("CreatedClientId"));

            _bus.PubSub.Subscribe<((Guid AccountId, Guid ClientId), CreditOperationRequest Request)>("CreditOperation_Core", tuple =>
            {
                var (Account, Request) = tuple;  

                using (var scope = _serviceProvider.CreateScope())
                {
                    var OperationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var AccountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    OperationService.MakeOperation(new CreditOperation(Request, AccountService.GetAccount(Account.AccountId, Account.ClientId)));
                }
            });

            _bus.Rpc.Respond<((Guid AccountId, Guid ClientId), CreditOperationRequest Request), ErrorResponse?>(tuple =>
            {
                var (Account, Request) = tuple;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var operationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    try
                    {
                        var account = accountService.GetAccount(Account.AccountId, Account.ClientId);
                        var operation = new CreditOperation(Request, account);
                        operationService.MakeOperation(operation);
                    }
                    catch (ErrorException ex) 
                    {
                        return new ErrorResponse(ex.status, ex.message);
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
