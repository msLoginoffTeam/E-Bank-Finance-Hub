using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Common.Trace;
using CreditService_Patterns.IServices;
using EasyNetQ;

namespace CreditService_Patterns.Services.Utils
{
    public class CreditRabbit : Common.Rabbit.RabbitMQ
    {
        Tracer _tracer;
        public CreditRabbit(IServiceProvider serviceProvider, Tracer tracer) : base(serviceProvider) 
        {
			_tracer = tracer;
        }

        public override void Configure()
        {
            RpcRespond<CreditCheckDTO, CreditCheckResponse>(request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    string traceId = request.TraceId;
                    var trace = _tracer.StartRequest(traceId, "RPC - CreditCheck", $"Request: {request.AccountId}");

					var CreditService = scope.ServiceProvider.GetRequiredService<ICreditService>();

                    if (CreditService.CheckIfHaveActiveCreditAsync(request.AccountId))
                    {
						_tracer.EndRequest(trace.DictionaryId, success: false, 404, "На счет не привязан кредит");
						return new CreditCheckResponse() { status = 404, message = "На счет не привязан кредит" };
                    }
                    else
                    {
						_tracer.EndRequest(trace.DictionaryId, success: true, 200);
						return new CreditCheckResponse()
                        {
                            status = 200,
                            message = ""
                        };
                    }
                }
            }, QueueName: "CreditCheck");
        }
    }
}
