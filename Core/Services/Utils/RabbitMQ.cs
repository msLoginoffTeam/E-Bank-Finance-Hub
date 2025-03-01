﻿using Core.Data.DTOs.Requests;
using Core.Data.Models;
using Core.Services.Utils.ErrorHandling;
using CoreApi.Models.innerModels;
using EasyNetQ;

namespace Core.Services.Utils
{
    public class RabbitMQ
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQ(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _bus = RabbitHutch.CreateBus("host=localhost");

            _bus.PubSub.Subscribe<Guid>("CreatedUserId_Core", ClientId =>
            {
                using (var Scope = _serviceProvider.CreateScope())
                {
                    AccountService AccountService = Scope.ServiceProvider.GetRequiredService<AccountService>();
                    AccountService.CreateClient(ClientId);
                }


            }, conf => conf.WithTopic("CreatedClientId"));

            _bus.PubSub.Subscribe<(Guid AccountId, CreditOperationRequest Request)>("CreditOperation_Core", tuple =>
            {
                var (AccountId, Request) = tuple;  

                using (var scope = _serviceProvider.CreateScope())
                {
                    var OperationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var AccountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    OperationService.MakeOperation(new CreditOperation(Request, AccountService.GetAccount(AccountId)));
                }
            });

            _bus.Rpc.Respond<(Guid AccountId, CreditOperationRequest Request), CustomException>(tuple =>
            {
                var (AccountId, Request) = tuple;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var operationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var account = accountService.GetAccount(AccountId);
                    var operation = new CreditOperation(Request, account);

                    try
                    {
                        operationService.MakeOperation(operation);
                    }
                    catch (ErrorException ex) 
                    {
                        return new CustomException(ex.message, "Make operation", "", ex.status);
                    }
                    catch (Exception ex)
                    {
                        return new CustomException("Some troubles", "Make operation", "", 500);
                    }
                    return new CustomException("Good", "Good", "Good", 200);
                }
            });
        }
    }
}
