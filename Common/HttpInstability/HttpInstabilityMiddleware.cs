
using Common.ErrorHandling;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Common.InternalServerErrorMiddleware
{
    public class HttpInstabilityMiddleware
    {
        private readonly RequestDelegate _next;
        private int SuccessRequestsCount = 1;
        private int TotalRequestsCount = 1;

        public HttpInstabilityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (DateTime.UtcNow.Minute % 2 != 0)
            {
                if ((double)SuccessRequestsCount / TotalRequestsCount < 0.5)
                {
                    TotalRequestsCount++;
                    await _next(context);
                    SuccessRequestsCount++;
                }
                else
                {
                    TotalRequestsCount++;
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(500, "Что-то пошло не так")));
                }
            }
            else
            {
                if ((double)SuccessRequestsCount / TotalRequestsCount < 0.9)
                {
                    TotalRequestsCount++;
                    await _next(context);
                    SuccessRequestsCount++;
                }
                else
                {
                    TotalRequestsCount++;
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(500, "Что-то пошло не так")));
                }
            }
        }
    }
}
