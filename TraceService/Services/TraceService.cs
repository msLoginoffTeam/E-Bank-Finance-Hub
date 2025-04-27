using Common.Trace;
using TraceService.Data.Models;
using TraceService.Data;
using TraceService.IServices;
using Microsoft.EntityFrameworkCore;

public class TraceBdService : ITraceBdService
{
	private readonly TraceContext _traceContext;
	private readonly ILogger<TraceBdService> _logger;

	public TraceBdService(TraceContext traceContext, ILogger<TraceBdService> logger)
	{
		_traceContext = traceContext ?? throw new ArgumentNullException(nameof(traceContext));
		_logger = logger;
	}

	public async Task<bool> AddTraceElement(LogEventModel data)
	{
		_logger.LogInformation($"Adding trace element: TraceId:{data.TraceId}, Event:{data.Event}...");

		var newLog = new TraceDbModel
		{
			Id = Guid.NewGuid(),
			TraceId = data.TraceId,
			Event = data.Event,
			Timestamp = data.Timestamp,
			Message = data.Message,
			Success = data.Success,
			Status = data.Status,
			Ms = data.Ms,
			RequestHeaders = data.RequestHeaders,
			RequestBody = data.RequestBody,
			ResponseBody = data.ResponseBody
		};

		try
		{
			await _traceContext.Traces.AddAsync(newLog);
			await _traceContext.SaveChangesAsync();

			_logger.LogInformation("Successfully added trace element with ID: {Id}", newLog.Id);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to save trace element: {Message}", ex.Message);
			if (ex.InnerException != null)
			{
				_logger.LogError("InnerException: {Inner}", ex.InnerException.Message);
			}
			throw;
		}
	}

	public async Task<List<TraceDbModel>> GetAllTracesAsync()
	{
		try
		{
			return await _traceContext.Traces.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при получении всех логов");
			return new List<TraceDbModel>();
		}
	}

}
