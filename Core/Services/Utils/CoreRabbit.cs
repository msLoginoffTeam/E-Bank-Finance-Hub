using Common.ErrorHandling;
using Common.Models;
using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Common.Trace;
using Core.Data.DTOs.Requests;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using EasyNetQ;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Core.Services.Utils
{
    public class CoreRabbit : Common.Rabbit.RabbitMQ
    {
		private readonly Tracer _tracer;

		public CoreRabbit(IServiceProvider serviceProvider, IConnectionMultiplexer redis, Tracer tracer) : base(serviceProvider, redis)
		{
			_tracer = tracer;
		}

		public override void Configure()
        {
			_bus.PubSub.Subscribe<CreatedUserIdMessage>("CreatedUserId_Core", message =>
			{
                string traceId = message.TraceId;
                var trace = _tracer.StartRequest(traceId, "RPC - CreatedClientId", $"Request: {message.ClientId}");

				try
				{
					using (var scope = _serviceProvider.CreateScope())
					{
						var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
						accountService.CreateClient(message.ClientId);
					}

					_tracer.EndRequest(trace.DictionaryId, success: true);
				}
				catch (Exception ex)
				{
					_tracer.EndRequest(traceId, success: false, responseBody: ex.Message);
				}
			}, conf => conf.WithTopic("CreatedClientId"));


			RpcRespond(IdempotencyWrapper<RabbitOperationRequest, RabbitResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    string traceId = Request.TraceId;
                    var trace = _tracer.StartRequest(traceId, "RPC - Operation", $"Request: {Request}");

					var operationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    object OperationResponse;

                    Operation Operation;
                    Account Account = accountService.GetAccount(Request.AccountId, Request.ClientId);

                    if (Request is CreditOperationRequest CreditRequest)
                    {
                        Operation = new CreditOperation(new OperationRequest(Request), Account, CreditRequest.CreditId, CreditRequest.Type, null);
                        OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                    }
                    else if (Request is TransferOperationRequest TransferOperationRequest)
                    {
                        Account ReceiverAccount = accountService.GetAccount(TransferOperationRequest.ReceiverAccountNumber);
                        if (ReceiverAccount == Account)
                        {
                            _tracer.EndRequest(trace.DictionaryId, success: false, 400, "Счета получателя и отправителя совпадают");
                            return new RabbitResponse(400, "Счета получателя и отправителя совпадают");
                        }

                        Operation = new TransferOperation(Account, new OperationRequest(Request), ReceiverAccount);
                        ((TransferOperation)Operation).ConvertedAmount = operationService.CountConvertedAmount(Account, ReceiverAccount, Operation.Amount);
                        Console.WriteLine(((TransferOperation)Operation).ConvertedAmount);
                        OperationResponse = new TransferOperationResponse(Operation as TransferOperation);
                    }
                    else
                    {
                        Operation = new CashOperation(new OperationRequest(Request), Account);
                        OperationResponse = new CashOperationResponse(Operation as CashOperation);
                    }
                    var res = operationService.MakeOperation(Operation);

                    if (Request is CreditOperationRequest CreRequest)
                    {
                        if (res == null)
                        {
                            Operation = new CreditOperation(new OperationRequest(Request), Account, CreRequest.CreditId, CreRequest.Type, true);
                            OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                        }
                        else
                        {
                            Operation = new CreditOperation(new OperationRequest(Request), Account, CreRequest.CreditId, CreRequest.Type, false);
                            OperationResponse = new CreditOperationResponse(Operation as CreditOperation);
                        }
                    }

                    if (res != null)
                    {
						_tracer.EndRequest(trace.DictionaryId, success: false, 403, "На счете не хватает денег для операции.");
						throw new ErrorException(403, "На счете не хватает денег для операции.");
                    }
					_tracer.EndRequest(trace.DictionaryId, success: true, 200);
					return new RabbitResponse(200, "");
                }
            }), QueueName: "Operations");

            RpcRespond<AccountExistRequest, RabbitResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    string traceId = Request.TraceId;
                    var trace = _tracer.StartRequest(traceId, "RPC - AccountExistCheck", $"Request: {Request}");

					var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var account = accountService.GetAccount(Request.AccountId, Request.ClientId);
                    if (account == null)
                    {
                        _tracer.EndRequest(trace.DictionaryId, success: false, 404);
                        return new RabbitResponse(404, "Счет не найден");
                    }
                    else if (account.IsClosed == true)
                    {
                        _tracer.EndRequest(trace.DictionaryId, success: false, 403);
                        return new RabbitResponse(403, "Счет закрыт");
                    }
                    else
                    {
                        _tracer.EndRequest(trace.DictionaryId, success: true, 200);
                        return new RabbitResponse(200, "");
                    }
                }
            }, QueueName: "AccountExistCheck");


            RpcRespond<GetRatingRequest, GetRatingResponse>(Request =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    string traceId = Request.TraceId;
                    var trace = _tracer.StartRequest(traceId, "RPC - GetRating", $"Request: {Request}");

					var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                    var client = accountService.GetClient(Request.ClientId);
					_tracer.EndRequest(trace.DictionaryId, success: true, 200);
					return new GetRatingResponse((int)client.Rating);
                }
            }, QueueName: "GetRating");
        }
    }
}
