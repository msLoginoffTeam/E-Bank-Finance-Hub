using Auth_Service.Data.DTOs;
using Auth_Service.Data.Models;
using Common.Rabbit.DTOs;
using Common.Trace;
using EasyNetQ;
using UserApi.Data.Models;

namespace Auth_Service.Services.Utils
{
    public class AuthRabbit : Common.Rabbit.RabbitMQ
    {
		private readonly Tracer _tracer;
		public AuthRabbit(IServiceProvider serviceProvider, Tracer tracer) : base(serviceProvider) 
        {
			_tracer = tracer;
		}

        public override void Configure()
        {
            _bus.PubSub.Subscribe<UserAuthDTO>("", UserAuthDTO =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    string traceId = UserAuthDTO.TraceId;
                    var trace = _tracer.StartRequest(traceId, "RPC - Register", $"UserAuthDTO: {UserAuthDTO}");
					AuthService AuthService = scope.ServiceProvider.GetRequiredService<AuthService>();

                    UserAuth UserAuth;
                    if (UserAuthDTO.Role == Role.Client.ToString())
                    {
                        UserAuth = new ClientAuth(UserAuthDTO);
                    }
                    else
                    {
                        UserAuth = new EmployeeAuth(UserAuthDTO);
                    }
					_tracer.EndRequest(trace.DictionaryId, success: true, 200);
					AuthService.Register(UserAuth);
                }
            });
        }
    }
}
