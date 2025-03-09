using Core_Api.Data.DTOs.Requests;
using CreditService_Patterns.IServices;
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

            _bus = RabbitHutch.CreateBus("host=rabbitmq");

            _bus.Rpc.Respond<Guid, bool>(AccountId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var CreditService = scope.ServiceProvider.GetRequiredService<ICreditService>();
                    
                    return CreditService.CheckIfHaveActiveCreditAsync(AccountId);
                }
            }, configure: x => x.WithQueueName("AccountCreditCheck"));
        }
    }
}
