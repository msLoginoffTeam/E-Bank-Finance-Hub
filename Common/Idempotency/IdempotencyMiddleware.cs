using Common.ErrorHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Common.Idempotency
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionMultiplexer _redis;

        public IdempotencyMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
        {
            _next = next;
            _redis = redis;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Delete)
            {
                var IdempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
                if (IdempotencyKey == null) throw new ErrorException(400, "Для таких операций необходим ключ идемпотентности");
                if (!Guid.TryParse(IdempotencyKey, out Guid guid))
                {
                    throw new ErrorException(400, "Ключ идемпотентности должен быть Guid");
                }

                var db = _redis.GetDatabase();
                var RequestResult = db.StringGet(IdempotencyKey);

                if (!RequestResult.IsNull)
                {
                    var Response = JsonSerializer.Deserialize<RequestResult>(RequestResult);

                    if (Response.RequestStatus == RequestStatus.Pending)
                    {
                        throw new ErrorException(500, "Подождите, заявка еще обрабатывается");
                    }
                    else
                    {
                        context.Response.StatusCode = Response.StatusCode;
                        context.Response.ContentType = Response.ContentType;
                        await context.Response.WriteAsync(Response.Body);
                    }
                }
                else
                {
                    db.StringSet(IdempotencyKey, JsonSerializer.Serialize(new RequestResult()), expiry: TimeSpan.FromMinutes(60));
                    Stream originalBody = context.Response.Body;

                    try
                    {
                        using (var memStream = new MemoryStream())
                        {
                            context.Response.Body = memStream;

                            await _next(context);

                            memStream.Position = 0;
                            string responseBody = new StreamReader(memStream).ReadToEnd();
                            if (context.Response.StatusCode != 500) db.StringSet(IdempotencyKey, JsonSerializer.Serialize(new RequestResult(context.Response.StatusCode, context.Response.ContentType, responseBody)), expiry: TimeSpan.FromMinutes(60));
                            else db.StringGetDelete(IdempotencyKey);

                            memStream.Position = 0;
                            await memStream.CopyToAsync(originalBody);
                        }
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }

    public class RequestResult
    {
        public RequestStatus RequestStatus { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
        public string Body { get; set; }

        public RequestResult()
        {
            this.RequestStatus = RequestStatus.Pending;
        }
        public RequestResult(int StatusCode, string ContentType, string Body)
        {
            this.RequestStatus = RequestStatus.Done;
            this.StatusCode = StatusCode;
            this.ContentType = ContentType;
            this.Body = Body;
        }
    }
    public enum RequestStatus
    {
        Pending,
        Done
    }
}
