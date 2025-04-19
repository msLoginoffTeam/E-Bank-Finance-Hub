using Common.ErrorHandling;
using Common.Idempotency;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using EasyNetQ;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Common.Rabbit
{
    public abstract class RabbitMQ
    {
        public readonly IBus _bus;
        protected readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;
        private readonly Random _random;

        private int SuccessRequestCount = 1;
        private int TotalRequestCount = 1;

        private bool State = true;

        public RabbitMQ(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") != null ? Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") : "host=localhost");
            _random = new Random();

            Configure();
        }

        public RabbitMQ(IServiceProvider serviceProvider, IConnectionMultiplexer redis) : this(serviceProvider)
        {
            _redis = redis;
        }

        public Func<TRequest, TResponse> InstabilityWrapper<TRequest, TResponse>(Func<TRequest, TResponse> Action) where TResponse : RabbitResponse
        {
            return (Request) =>
            {
                RabbitResponse Response;
                double FailureProbability = DateTime.UtcNow.Minute % 2 != 0 ? 0.5 : 0.9;

                if (_random.NextDouble() < FailureProbability)
                {
                    Response = new RabbitResponse(500, "Что-то пошло не так");
                }
                else
                {
                    Response = Action(Request);
                }

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

        public TResponse RpcRequest<TRequest, TResponse>(TRequest Request, string QueueName) where TResponse : RabbitResponse
        {
            if (State)
            {
                TResponse Response = _bus.Rpc.Request<TRequest, TResponse>(Request, configure => configure.WithQueueName(QueueName));
                TimeSpan RetryTime = TimeSpan.FromSeconds(1);

                while (Response?.status == 500)
                {
                    if (State)
                    {
                        Thread.Sleep(RetryTime);
                        RetryTime = TimeSpan.FromSeconds(Math.Min(RetryTime.Seconds * 2, 30));
                        TotalRequestCount++;

                        if ((double)SuccessRequestCount / TotalRequestCount < 0.3)
                        {
                            State = false;
                        }
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(30));
                    }
                    Response = _bus.Rpc.Request<TRequest, TResponse>(Request, configure => configure.WithQueueName(QueueName));
                }
                State = true;
                SuccessRequestCount++;
                TotalRequestCount++;
                return Response;
            }
            else
            {
                return new RabbitResponse(500, "Сервис временно недоступен") as TResponse;
            }
        }

        protected void RpcRespond<TRequest, TResponse>(Func<TRequest, TResponse> Action, string QueueName) where TResponse : RabbitResponse
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
            }, configure => configure.WithQueueName(QueueName));
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
