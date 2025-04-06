using CreditService_Patterns.IServices;
using EasyNetQ;

namespace CreditService_Patterns.Services.Utils
{
    public class CreditRabbit : Common.Rabbit.RabbitMQ
    {
        public CreditRabbit(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override void Configure()
        {
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
