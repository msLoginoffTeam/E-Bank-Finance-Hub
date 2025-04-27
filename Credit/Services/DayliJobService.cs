using Quartz;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CreditService_Patterns.IServices;
using Common.Trace;
using Common.Rabbit.DTOs.Responses;

public class DailyJobService : IJob
{
    private readonly ILogger<DailyJobService> _logger;
    private readonly ICreditService _creditService;
    private readonly Tracer _tracer;

    public DailyJobService(ILogger<DailyJobService> logger, ICreditService creditService, Tracer tracer)
    {
        _logger = logger;
        _creditService = creditService;
        _tracer = tracer;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Фоновая задача запущена в {DateTime.UtcNow}");

        try
        {
			//try
			//{
			//    await _creditService.PercentAsync();
			//    _logger.LogInformation("Фоновая задача по обновлению процентов выполнена успешно.");
			//}
			//catch (Exception ex)
			//{
			//    _logger.LogError($"Ошибка в методе PercentAsync: {ex.Message}");
			//}

			var trace = _tracer.StartRequest(null, "DailyJobService - PayOffTheLoanAutomaticAsync");
			try
            {
				await _creditService.PayOffTheLoanAutomaticAsync(trace.TraceId);
                _tracer.EndRequest(trace.DictionaryId, true, 200);
				_logger.LogInformation("Фоновая задача по погашению кредита выполнена успешно.");
            }
            catch (Exception ex)
            {
				_tracer.EndRequest(trace.DictionaryId, true, 500);
				_logger.LogError($"Ошибка в методе PayOffTheLoanAutomaticAsync: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при выполнении фоновой задачи: {ex.Message}");
        }

        _logger.LogInformation("Фоновая задача завершена");
    }
}
