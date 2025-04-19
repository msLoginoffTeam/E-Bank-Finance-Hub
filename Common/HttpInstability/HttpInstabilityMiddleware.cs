
using Common.ErrorHandling;
using Common.Rabbit.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace Common.InternalServerErrorMiddleware
{
    public class HttpInstabilityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Random _random;

        public HttpInstabilityMiddleware(RequestDelegate next)
        {
            _next = next;
            _random = new Random();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            double FailureProbability = DateTime.UtcNow.Minute % 2 != 0 ? 0.5 : 0.9;

            if (_random.NextDouble() < FailureProbability)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(500, "Что-то пошло не так")));
            }
            else
            {
                await _next(context);
            }
        }
    }
}
