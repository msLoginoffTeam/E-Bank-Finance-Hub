using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Common.ErrorHandling;

namespace Auth_Service.Services.Utils
{
	public class GlobalExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<GlobalExceptionFilter> _logger;

		// Внедрение зависимости ILogger в конструктор
		public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			if (context.ExceptionHandled) return;

			// Логирование информации о возникшей ошибке
			_logger.LogError(context.Exception, "An exception occurred in the GlobalExceptionFilter.");

			// В зависимости от типа ошибки создаём сообщение
			string message = context.Exception is ErrorException errorException
				? errorException.message
				: "Unexpected error";

			// Логируем сообщение, которое будет отправлено пользователю
			_logger.LogInformation("Redirecting to Error page with message: {Message}", message);

			// Устанавливаем результат (перенаправление на страницу ошибки)
			context.Result = new RedirectToActionResult("Error", "Auth", new { message });

			// Отмечаем, что исключение обработано
			context.ExceptionHandled = true;
		}
	}
}
