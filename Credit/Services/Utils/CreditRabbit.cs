using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using CreditService_Patterns.IServices;
using EasyNetQ;

namespace CreditService_Patterns.Services.Utils
{
    public class CreditRabbit : Common.Rabbit.RabbitMQ
    {
        public CreditRabbit(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override void Configure()
        {
            RpcRespond<Guid, CreditCheckResponse>(AccountId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var CreditService = scope.ServiceProvider.GetRequiredService<ICreditService>();

                    if (CreditService.CheckIfHaveActiveCreditAsync(AccountId)) return new CreditCheckResponse() { status = 404, message = "На счет не привязан кредит" };
                    else return new CreditCheckResponse()
                    {
                        status = 200, 
                        message = ""
                    };
                }
            });
        }
    }
}
