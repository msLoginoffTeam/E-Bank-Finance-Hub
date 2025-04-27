using Common.Rabbit.DTOs.Responses;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using EasyNetQ;
using Microsoft.AspNetCore.Http.Headers;

namespace Common.Trace
{
	public class Tracer
	{
		private static ConcurrentDictionary<string, RequestInfoModel> Requests = new();
		private readonly ILogger<Tracer> _logger;

		public Tracer(ILogger<Tracer> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public TraceElement StartRequest(string? traceId, string requestHeaders = null, string requestBody = null)
		{
			var TraceId = traceId == null ? Guid.NewGuid().ToString() : traceId;
			var DictionaryId = Guid.NewGuid().ToString();
			var info = new RequestInfoModel
			{
				TraceId = TraceId,
				StartTime = Stopwatch.StartNew(),
				RequestHeaders = requestHeaders,
				RequestBody = requestBody
			};
			Requests[DictionaryId] = info;

			SendLog(new LogEventModel
			{
				TraceId = TraceId,
				Event = traceId == null ? "StartRequest" : "ContinueRequest",
				Timestamp = DateTime.UtcNow,
				Message = traceId == null ?  "Request started" : "Request sent",
				RequestHeaders = requestHeaders,
				RequestBody = requestBody
			});

			return new TraceElement() { TraceId = TraceId, DictionaryId = DictionaryId};
		}

		public void EndRequest(string dictionaryId, bool success, int responseStatusCode = 0, string responseBody = null)
		{
			if (Requests.TryRemove(dictionaryId, out var info))
			{
				info.StartTime.Stop();
				info.Success = success;
				info.ResponseStatusCode = responseStatusCode;
				info.ResponseBody = responseBody;

				SendLog(new LogEventModel
				{
					TraceId = info.TraceId,
					Event = "EndRequest",
					Timestamp = DateTime.UtcNow,
					Message = $"Request ended. Success: {success}. Status: {responseStatusCode}. Duration: {info.StartTime.ElapsedMilliseconds}ms",
					Success = success,
					Status = responseStatusCode,
					Ms = info.StartTime.ElapsedMilliseconds,
					RequestHeaders = info.RequestHeaders,
					RequestBody = info.RequestBody,
					ResponseBody = info.ResponseBody
				});
			}
		}

		public void LogEvent(string traceId, string message)
		{
			SendLog(new LogEventModel
			{
				TraceId = traceId,
				Event = "Event",
				Timestamp = DateTime.UtcNow,
				Message = message
			});
		}

		private void SendLog(LogEventModel log)
		{
			try
			{
				using (var bus = RabbitHutch.CreateBus("host=rabbitmq"))
				{
					_logger.LogInformation($"Trying to send LogInfo: TraceId:{log.TraceId}, Event:{log.Event}, Timestamp:{log.Timestamp}, Message:{log.Message}, Success:{log.Success}, Status:{log.Status}, Ms:{log.Ms}, RequestHeaders:{log.RequestHeaders}, RequestBody:{log.RequestBody}, ResponseBody:{log.ResponseBody}");

					var response = bus.Rpc.Request<LogEventModel, RabbitResponse>(log, x => x.WithQueueName("MonitoringLogSaveQueue"));

					if (response.status != 200)
					{
						_logger.LogInformation($"[WARN] Failed to send log: {response.message}");
					}
					else
					{
						_logger.LogInformation($"[INFO] Successfully sent log: {log.TraceId}");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"[ERROR] Exception occurred while sending log: {ex.Message}");
			}
		}
	}

	public class RequestInfoModel
	{
		public string TraceId { get; set; }
		public Stopwatch StartTime { get; set; }
		public bool? Success { get; set; }
		public string RequestHeaders { get; set; }
		public string RequestBody { get; set; }
		public int ResponseStatusCode { get; set; }
		public string ResponseBody { get; set; }
	}

	public class LogEventModel
	{
		public string TraceId { get; set; }
		public string Event { get; set; }
		public DateTime Timestamp { get; set; }
		public string Message { get; set; }
		public bool? Success { get; set; }
		public int? Status {get; set; }
		public long? Ms { get; set; } 
		public string? RequestHeaders { get; set; }
		public string? RequestBody { get; set; }
		public string? ResponseBody { get; set; }
	}
}
