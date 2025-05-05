using Common.Data.DTOs;
using Common.ErrorHandling;
using Common.Rabbit.DTOs.Responses;
using UserApi.Data.Models;
using Common.Rabbit.DTOs.Requests;
using Common.Trace;

namespace UserApi.Services.Utils
{
    public class UserRabbit : Common.Rabbit.RabbitMQ
    {
		private readonly Tracer _tracer;
		private readonly ILogger<UserRabbit> _logger;

		public UserRabbit(IServiceProvider serviceProvider, Tracer tracer, ILogger<UserRabbit> logger) : base(serviceProvider)
		{
			_tracer = tracer;
			_logger = logger;
		}

		public override void Configure()
        {
            RpcRespond<UserInfo, UserInfoResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
					var trace = _tracer.StartRequest(Request.TracerId, "RPC - UserInfoById", $"Request: {Request}");

					UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();
                    User User = UserService.GetUserById(Request.UserId);

					_tracer.EndRequest(trace.DictionaryId, success: true, 200);
					return new UserInfoResponse()
                    {
                        Id = User.Id,
                        Email = User.Email,
                        Roles = User.Roles.Select(UserRole => UserRole.Role.ToString()).ToList(),
                        IsBlocked = User.IsBlocked,
                    };
                }
            }, QueueName: "UserInfoById");

			RpcRespond<UserInfoEmail, UserInfoResponse>(Request =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var trace = _tracer.StartRequest(Request.TraceId, "RPC - UserInfoByEmail", $"Request: {Request}");

					UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

					UserInfoResponse UserInfo;
					try
					{
						User User = UserService.GetUserByLogin(Request.Email);

						UserInfo = new UserInfoResponse()
						{
							Id = User.Id,
							Email = User.Email,
							Roles = User.Roles.Select(UserRole => UserRole.Role.ToString()).ToList(),
							IsBlocked = User.IsBlocked
						};
					}
					catch (ErrorException ex)
					{
						UserInfo = new UserInfoResponse()
						{
							status = ex.status,
							message = ex.message,
						};
					}

					_tracer.EndRequest(trace.DictionaryId, success: true, 200);

					return UserInfo;
				}
			}, QueueName: "UserInfoByEmail");


			RpcRespond<string, EmployeeDeviceTokensResponse>(_ =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
					var trace = _tracer.StartRequest(null, "RPC - EmployeeDeviceToken");

					UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();

					_tracer.EndRequest(trace.DictionaryId, success: true, 200);
					return new EmployeeDeviceTokensResponse(UserService.GetEmployeeDeviceTokens());
                }
            }, QueueName: "EmployeeDeviceToken");

            RpcRespond<Guid, ClientDeviceTokenResponse>(ClientId =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
					var trace = _tracer.StartRequest(null, "RPC - ClientDeviceToken", $"Request: {ClientId}");
					UserService UserService = scope.ServiceProvider.GetRequiredService<UserService>();
					var ClientDeviceToken = UserService.GetClientDeviceToken(ClientId);
					_tracer.EndRequest(trace.DictionaryId, success: true, 200);
					return new ClientDeviceTokenResponse(UserService.GetClientDeviceToken(ClientId));
                }
            }, QueueName: "ClientDeviceToken");
        }
    }
}
