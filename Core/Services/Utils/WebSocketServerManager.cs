using Common;
using Common.ErrorHandling;
using Core.Data.DTOs.Responses;
using Core.Data.Models;
using Core.Services;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Core_Api.Services.Utils
{
    public class WebSocketServerManager
    {
        private WebSocketServer _server { get; set; }
        private List<ClientOperationsBroadcast> _clients { get; set; }

        private readonly IServiceProvider _serviceProvider;
        private Common.CreditOperationType Configuration { get; set; }

        public WebSocketServerManager(Common.CreditOperationType Configuration, IServiceProvider serviceProvider)
        {
            this.Configuration = Configuration;
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            _clients = new List<ClientOperationsBroadcast>();
            _server = new WebSocketServer("ws://0.0.0.0:5009");

            _server.Start(Socket =>
            {
                Socket.OnOpen = () =>
                {
                    var User = AuthenticationExtensions.ValidateToken(Socket.ConnectionInfo.Headers["Authorization"].Substring(7), Configuration);
                    if (User == null) throw new ErrorException(401, "Аутентификация не прошла, передайте валидный токен");

                    Guid AccountId = new Guid(Socket.ConnectionInfo.Path.Substring(1));
                    Guid UserId = new Guid(User.Claims.ToList().First().Value);
                    string Role = User.Claims.ToList()[2].Value;

                    Account Account;
                    List<Operation> Operations;
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        AccountService AccountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                        Account = AccountService.GetAccount(AccountId);
                        if (Role == "Client" && Account.Client.Id != UserId)
                        {
                            throw new ErrorException(403, "Счет не принадлежит клиенту.");
                        }

                        OperationService OperationService = scope.ServiceProvider.GetRequiredService<OperationService>();
                        Operations = OperationService.GetOperationsByAccountId(AccountId);
                    }

                    List<object> Response = new List<object>();
                    foreach (var Operation in Operations)
                    {
                        switch (Operation.OperationCategory)
                        {
                            case OperationCategory.Credit:
                                {
                                    Response.Add(new CreditOperationResponse(Operation as CreditOperation));
                                    break;
                                }
                            case OperationCategory.Cash:
                                {
                                    Response.Add(new CashOperationResponse(Operation as CashOperation));
                                    break;
                                }
                            case OperationCategory.Transfer:
                                {
                                    Response.Add(new TransferOperationResponse(Operation as TransferOperation));
                                    break;
                                }
                        }
                    }

                    ClientOperationsBroadcast? Broadcast = _clients.FirstOrDefault(ClientBroadcast => ClientBroadcast.AccountId == AccountId);
                    if (Broadcast != null)
                    {
                        Broadcast.Receivers.Add(Socket);
                    }
                    else
                    {
                        _clients.Add(new ClientOperationsBroadcast(AccountId, Socket));
                    }

                    var jsonMessage = JsonSerializer.Serialize(Response, new JsonSerializerOptions()
                    {
                        Converters = { new JsonStringEnumConverter() }
                    });

                    var Sockets = GetBroadcast(Account.Id);
                    foreach (var Socket in Sockets)
                    {
                        Socket.Send(jsonMessage);
                    }
                };

                Socket.OnClose = () =>
                {
                    var User = AuthenticationExtensions.ValidateToken(Socket.ConnectionInfo.Headers["Authorization"].Substring(7), Configuration);

                    Guid AccountId = new Guid(Socket.ConnectionInfo.Path.Substring(1));
                    Guid UserId = new Guid(User.Claims.ToList().First().Value);
                    string Role = User.Claims.ToList()[2].Value;

                    ClientOperationsBroadcast? Broadcast = _clients.FirstOrDefault(OperationBroadcast => OperationBroadcast.AccountId == AccountId);
                    if (Broadcast != null)
                    {
                        Broadcast.Receivers.Remove(Socket);
                        if (Broadcast.Receivers.Count < 1)
                        {
                            _clients.Remove(Broadcast);
                        }
                    }
                };
            });
        }

        public List<IWebSocketConnection>? GetBroadcast(Guid AccountId)
        {
            return _clients.FirstOrDefault(OperationBroadcast => OperationBroadcast.AccountId == AccountId)?.Receivers;
        }

        public void Stop()
        { 
            _server?.Dispose();
        }
    }

    public class ClientOperationsBroadcast
    {
        public List<IWebSocketConnection> Receivers { get; set; }
        public Guid AccountId { get; set; }

        public ClientOperationsBroadcast(Guid AccountId, IWebSocketConnection Socket)
        {
            this.AccountId = AccountId;
            Receivers = new List<IWebSocketConnection>() { Socket };
        }
    }
}
