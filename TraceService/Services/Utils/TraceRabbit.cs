using Microsoft.Extensions.Logging;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using TraceService.Services.Utils;
using Common.Rabbit;
using Common.Trace;
using TraceService.IServices;
using EasyNetQ;

namespace TraceService.Services.Utils
{
	public class TraceRabbit : Common.Rabbit.RabbitMQ
	{
		private readonly ILogger<TraceRabbit> _logger;
		public TraceRabbit(IServiceProvider serviceProvider, ILogger<TraceRabbit> logger) : base(serviceProvider)
		{
			_logger = logger;
		}

		public override void Configure()
		{
			_bus.Rpc.Respond<LogEventModel, RabbitResponse>(async Request =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					_logger.LogInformation("Received request to save trace with event: {Event}", Request.Event);
					var accountService = scope.ServiceProvider.GetRequiredService<ITraceBdService>();

					try
					{
						_logger.LogInformation($"Trying to add LogInfo: TraceId:{Request.TraceId}, Event:{Request.Event}, Timestamp:{Request.Timestamp}, Message:{Request.Message}, Success:{Request.Success}, Status:{Request.Status}, Ms:{Request.Ms}, RequestHeaders:{Request.RequestHeaders}, RequestBody:{Request.RequestBody}, ResponseBody:{Request.ResponseBody}");
						var result = await accountService.AddTraceElement(Request);
						_logger.LogInformation("Successfully added trace element for event: {Event}", Request.Event);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error occurred while adding trace element for event: {Event}", Request.Event);
					}
					return new RabbitResponse();
				}
			}, configure: x => x.WithQueueName("MonitoringLogSaveQueue"));
		}
	}
}
