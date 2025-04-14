using Common.Data.DTOs;
using Common.ErrorHandling;
using EasyNetQ;
using UserApi.Data.Models;

namespace UserApi.Services.Utils
{
    public class UserRabbit : Common.Rabbit.RabbitMQ
    {
        public UserRabbit(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override void Configure()
        {
            _bus.Rpc.Respond<Guid, UserInfoResponse>(UserId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();
                    User User = UserService.GetUserById(UserId);

                    return new UserInfoResponse()
                    {
                        Id = User.Id,
                        Email = User.Email,
                        Roles = User.Roles.Select(UserRole => UserRole.Role.ToString()).ToList(),
                        IsBlocked = User.IsBlocked,
                    };
                }
            });

            _bus.Rpc.Respond<string, UserInfoResponse>(Email =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

                    UserInfoResponse UserInfo;
                    try
                    {
                        User User = UserService.GetUserByLogin(Email);
                        UserInfo = new UserInfoResponse()
                        {
                            Id = User.Id,
                            Email = User.Email,
                            Roles = User.Roles.Select(UserRole => UserRole.Role.ToString()).ToList(),
                            IsBlocked = User.IsBlocked
                        };
                    }
                    catch(ErrorException ex)
                    {
                        UserInfo = new UserInfoResponse()
                        {
                            error = ex.message,
                        };
                    }

                    return UserInfo;
                }
            });

            _bus.Rpc.Respond<string, List<string>>(_ =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

                    return UserService.GetEmployeeDeviceTokens();
                }
            }, x => x.WithQueueName("GetEmployeeDeviceTokens"));

            _bus.Rpc.Respond<Guid, string>(ClientId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

                    return UserService.GetClientDeviceToken(ClientId);
                }
            }, x => x.WithQueueName("GetClientDeviceToken"));
        }
    }
}
