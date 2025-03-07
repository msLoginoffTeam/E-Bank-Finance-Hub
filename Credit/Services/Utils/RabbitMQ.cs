using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services.Utils.ErrorHandling;
using CoreApi.Models.innerModels;
using EasyNetQ;

namespace CreditService_Patterns.Services.Utils
{
    public class RabbitMQ
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQ(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _bus = RabbitHutch.CreateBus("host=localhost");

            _bus.Rpc.RespondAsync<Guid, bool>(AccountId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var CreditService = scope.ServiceProvider.GetRequiredService<CreditService>();

                    return CreditService.CheckIfHaveActiveCreditAsync(AccountId);
                }
            });
        }
    }
}
