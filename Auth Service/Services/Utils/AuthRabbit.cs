using Auth_Service.Data.DTOs;
using Auth_Service.Data.Models;
using Common.Rabbit.DTOs;
using EasyNetQ;
using UserApi.Data.Models;

namespace Auth_Service.Services.Utils
{
    public class AuthRabbit : Common.Rabbit.RabbitMQ
    {
        public AuthRabbit(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override void Configure()
        {
            _bus.PubSub.Subscribe<UserAuthDTO>("", UserAuthDTO =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
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
                    AuthService.Register(UserAuth);
                }
            });
        }
    }
}
