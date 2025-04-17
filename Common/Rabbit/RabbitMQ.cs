using Common.ErrorHandling;
using Common.Idempotency;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using EasyNetQ;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Common.Rabbit
{
    public abstract class RabbitMQ
    {
        public readonly IBus _bus;
        protected readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;
        private int SuccessRequestsCount = 1;
        private int TotalRequestsCount = 1;

        public RabbitMQ(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") != null ? Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") : "host=localhost");

            Configure();
        }

        public RabbitMQ(IServiceProvider serviceProvider, IConnectionMultiplexer redis)
        {
            _serviceProvider = serviceProvider;
            _bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") != null ? Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") : "host=localhost");
            _redis = redis;

            Configure();
        }

        public Func<TRequest, TResponse> InstabilityWrapper<TRequest, TResponse>(Func<TRequest, TResponse> Action) where TResponse : RabbitResponse
        {
            return (Request) =>
            {
                RabbitResponse Response;
                if (DateTime.UtcNow.Minute % 2 != 0)
                {
                    if ((double)SuccessRequestsCount / TotalRequestsCount < 0.5)
                    {
                        Response = Action(Request);
                    }
                    else
                    {
                        Response = new RabbitResponse(500, "Что-то пошло не так");
                    }
                }
                else
                {
                    if ((double)SuccessRequestsCount / TotalRequestsCount < 0.9)
                    {
                        Response = Action(Request);
                    }
                    else
                    {
                        Response = new RabbitResponse(500, "Что-то пошло не так");
                    }
                }

                if (Response.status != 500)
                {
                    SuccessRequestsCount++;
                }
                TotalRequestsCount++;

                return Response as TResponse;
            };
        }

        public Func<TRequest, TResponse> IdempotencyWrapper<TRequest, TResponse>(Func<TRequest, TResponse> Action) where TRequest : RabbitRequest where TResponse : RabbitResponse
        {
            return (Request) =>
            {
                Guid IdempotencyKey = Request.IdempotencyKey;
                if (IdempotencyKey == null) throw new ErrorException(400, "Для таких операций необходим ключ идемпотентности");

                var db = _redis.GetDatabase();
                var RequestResult = db.StringGet(IdempotencyKey.ToString());

                if (!RequestResult.IsNull)
                {
                    var Response = JsonSerializer.Deserialize<RabbitRequestResult>(RequestResult);

                    if (Response.RequestStatus == RequestStatus.Pending)
                    {
                        return new RabbitResponse(500, "Заявка еще в обработке") as TResponse;
                    }
                    else return JsonSerializer.Deserialize<TResponse>(Response.Response);
                }
                else
                {
                    db.StringSet(IdempotencyKey.ToString(), JsonSerializer.Serialize(new RabbitRequestResult()));

                    RabbitResponse Response;
                    try
                    {
                        Response = Action(Request);
                    }
                    catch (ErrorException ex)
                    {
                        return new RabbitResponse(ex) as TResponse;
                    }

                    if (Response.status != 500) db.StringSet(IdempotencyKey.ToString(), JsonSerializer.Serialize(new RabbitRequestResult(JsonSerializer.Serialize(Response))));
                    else db.StringGetDelete(IdempotencyKey.ToString());

                    return Response as TResponse;
                }
            };
        }

        protected void RpcRespond<TRequest, TResponse>(Func<TRequest, TResponse> Action) where TResponse : RabbitResponse
        {
            _bus.Rpc.Respond<TRequest, TResponse>(Request =>
            {
                Func<TRequest, TResponse> RespondFunction;
                if (Environment.GetEnvironmentVariable("USE_INSTABILITY") == "true")
                {
                    RespondFunction = InstabilityWrapper(Action);
                }
                else
                {
                    RespondFunction = Action;
                }
  
                return RespondFunction(Request);
            });
        }


        //protected TResponse ExecuteWithIdempotency<TRequest, TResponse>(Guid IdempotencyKey, TRequest Request, Func<TRequest, TResponse> Action) where TResponse : RabbitResponse
        //{
        //    var db = _redis.GetDatabase();
        //    var RequestResult = db.StringGet(IdempotencyKey.ToString());

        //    if (!RequestResult.IsNull)
        //    {
        //        var Response = JsonSerializer.Deserialize<RabbitRequestResult>(RequestResult);

        //        if (Response.RequestStatus == RequestStatus.Pending)
        //        {
        //            return new RabbitResponse(500, "Заявка еще в обработке") as TResponse;
        //        }
        //        else
        //        {
        //            return JsonSerializer.Deserialize<TResponse>(Response.Response);
        //        }
        //    }
        //    else
        //    {
        //        db.StringSet(IdempotencyKey.ToString(), JsonSerializer.Serialize(new RabbitRequestResult()));

        //        RabbitResponse Response;
        //        try
        //        {
        //            Response = Action(Request);
        //        }
        //        catch (ErrorException ex)
        //        {
        //            return new RabbitResponse(ex) as TResponse;
        //        }

        //        if (Response.status != 500) db.StringSet(IdempotencyKey.ToString(), JsonSerializer.Serialize(new RabbitRequestResult(JsonSerializer.Serialize(Response))));
        //        else db.StringGetDelete(IdempotencyKey.ToString());

        //        return Response as TResponse;
        //    }
        //}


        //protected void InstableRpc<TRequest, TResponse>(Func<TRequest, TResponse> handler) where TResponse : RabbitResponse
        //{
        //    _bus.Rpc.Respond<TRequest, TResponse>(Request =>
        //    {
        //        RabbitResponse Response;
        //        if (DateTime.UtcNow.Minute % 2 != 0)
        //        {
        //            if ((double)SuccessRequestsCount / TotalRequestsCount < 0.5)
        //            {
        //                Response = handler(Request);
        //            }
        //            else
        //            {
        //                Response = new RabbitResponse(500, "Что-то пошло не так");
        //            }
        //        }
        //        else
        //        {
        //            if ((double)SuccessRequestsCount / TotalRequestsCount < 0.9)
        //            {
        //                Response = handler(Request);
        //            }
        //            else
        //            {
        //                Response = new RabbitResponse(500, "Что-то пошло не так");
        //            }
        //        }

        //        if (Response.status != 500)
        //        {
        //            SuccessRequestsCount++;
        //        }
        //        TotalRequestsCount++;

        //        return Response as TResponse;
        //    });
        //}

        public abstract void Configure();
    }

    public class RabbitRequestResult
    {
        public RequestStatus RequestStatus { get; set; }
        public string Response { get; set; }

        public RabbitRequestResult()
        {
            RequestStatus = RequestStatus.Pending;
        }

        public RabbitRequestResult(string Response)
        {
            RequestStatus |= RequestStatus.Done;
            this.Response = Response;
        }
    }
}
