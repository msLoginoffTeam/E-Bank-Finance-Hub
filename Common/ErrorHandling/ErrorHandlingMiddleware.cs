using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Common.ErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ErrorException exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, ErrorException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.status;

            var result = JsonSerializer.Serialize(new ErrorResponse(exception.status, exception.message));
            return context.Response.WriteAsync(result);
        }
    }
}
