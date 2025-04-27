using Common.Data.DTOs;
using Common.ErrorHandling;
using Common.Rabbit.DTOs.Responses;
using UserApi.Data.Models;

namespace UserApi.Services.Utils
{
    public class UserRabbit : Common.Rabbit.RabbitMQ
    {
        public UserRabbit(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override void Configure()
        {
            RpcRespond<Guid, UserInfoResponse>(UserId =>
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
            }, QueueName: "UserInfoById");

            RpcRespond<string, UserInfoResponse>(Email =>
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
                            status = ex.status,
                            message = ex.message,
                        };
                    }

                    return UserInfo;
                }
            }, QueueName: "UserInfoByEmail");

            RpcRespond<string, EmployeeDeviceTokensResponse>(_ =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

                    return new EmployeeDeviceTokensResponse(UserService.GetEmployeeDeviceTokens());
                }
            }, QueueName: "EmployeeDeviceToken");

            RpcRespond<Guid, ClientDeviceTokenResponse>(ClientId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

                    var ClientDeviceToken = UserService.GetClientDeviceToken(ClientId);

                    return new ClientDeviceTokenResponse(ClientDeviceToken);
                }
            }, QueueName: "ClientDeviceToken");
        }
    }
}
