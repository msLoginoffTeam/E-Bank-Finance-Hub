using Quartz;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CreditService_Patterns.IServices;

public class DailyJobService : IJob
{
    private readonly ILogger<DailyJobService> _logger;
    private readonly ICreditService _creditService;

    public DailyJobService(ILogger<DailyJobService> logger, ICreditService creditService)
    {
        _logger = logger;
        _creditService = creditService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Фоновая задача запущена в {DateTime.UtcNow}");

        try
        {
            try
            {
                await _creditService.PayOffTheLoanAutomaticAsync();
                _logger.LogInformation("Фоновая задача по погашению кредита выполнена успешно.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе PayOffTheLoanAutomaticAsync: {ex.Message}");
            }

            try
            {
                await _creditService.PercentAsync();
                _logger.LogInformation("Фоновая задача по обновлению процентов выполнена успешно.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе PercentAsync: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при выполнении фоновой задачи: {ex.Message}");
        }

        _logger.LogInformation("Фоновая задача завершена");
    }
}
