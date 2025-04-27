using Common.Rabbit.DTOs.Requests;
using Common.Rabbit.DTOs.Responses;
using Core.Data.Models;
using Core.Services.Utils;
using EasyNetQ;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Core_Api.Services.Utils
{
    public class FirebaseNotificator : IHostedService, IDisposable
    {
        private readonly HttpClient _client;
        private static List<Notification> _employeeNotifications = new List<Notification>();
        private static List<(Notification Notification, Guid ClientId)> _clientNotifications = new List<(Notification Notification, Guid ClientId)>();
        private Timer _timer;
        private readonly CoreRabbit _rabbit;

        public FirebaseNotificator(CoreRabbit rabbit)
        {
            _client = new HttpClient();
            _rabbit = rabbit;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ =>
            {
                Task ClientNotifications = null;
                Task EmployeeNotifications = null;

                if (!_clientNotifications.IsNullOrEmpty())
                {
                    ClientNotifications = ProcessClientNotifications();
                }
                if (!_employeeNotifications.IsNullOrEmpty())
                {
                    EmployeeNotifications = ProcessEmployeeNotifications();
                }
                if (ClientNotifications != null) await ClientNotifications;
                if (EmployeeNotifications != null) await EmployeeNotifications;

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {  
            _client?.Dispose();
            _timer?.Dispose();
        }

        public static void AddNotificationReceiversFromOperation(Operation Operation)
        {
            Notification EmployeeNotification;
            Notification ClientNotification;

            if (Operation is CreditOperation CreditOperation)
            {
                if (CreditOperation.OperationType == OperationType.Income)
                {
                    EmployeeNotification = new Notification("Кредит взят", $"На счет {Operation.TargetAccount.Number} клиента {Operation.TargetAccount.Client.Id} начислено {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                    ClientNotification = new Notification("Кредит успешно взят", $"На счет {Operation.TargetAccount.Number} начислено {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                }
                else
                {
                    EmployeeNotification = new Notification("Оплата кредита", $"Со счета {Operation.TargetAccount.Number} клиента {Operation.TargetAccount.Client.Id} снято {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                    ClientNotification = new Notification("Оплата кредита", $"Со счета {Operation.TargetAccount.Number} снято {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                }
            }
            else if (Operation is TransferOperation TransferOperation)
            {
                EmployeeNotification = new Notification("Перевод между клиентами", $"Со счета {TransferOperation.SenderAccount.Number} на счет {Operation.TargetAccount.Number} переведено {TransferOperation.ConvertedAmount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                ClientNotification = new Notification("Вам поступил перевод", $"На счет {Operation.TargetAccount.Number} поступило {TransferOperation.ConvertedAmount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
            }
            else
            {
                if (Operation.OperationType == OperationType.Income)
                {
                    EmployeeNotification = new Notification("Деньги зачислены", $"На счет {Operation.TargetAccount.Number} клиента {Operation.TargetAccount.Client.Id} начислено {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                    ClientNotification = new Notification("Деньги зачислены", $"На счет {Operation.TargetAccount.Number} начислено {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                }
                else
                {
                    EmployeeNotification = new Notification("Деньги сняты", $"Со счета {Operation.TargetAccount.Number} клиента {Operation.TargetAccount.Client.Id} снято {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                    ClientNotification = new Notification("Деньги сняты", $"Со счета {Operation.TargetAccount.Number} снято {Operation.Amount} {(Operation.TargetAccount.Currency == Currency.Ruble ? "руб." : Operation.TargetAccount.Currency == Currency.Euro ? "евро" : "долл.")}");
                }
            }

            _employeeNotifications.Add(EmployeeNotification);
            _clientNotifications.Add((ClientNotification, (Guid)Operation.TargetAccount.Client.Id!));

        }

        private async Task ProcessClientNotifications()
        {
            var tmp = _clientNotifications.ToList();
            _clientNotifications.Clear();

            foreach (var ClientNotification in tmp)
            {
                ClientDeviceTokenResponse ClientDeviceToken = _rabbit.RpcRequest<Guid, ClientDeviceTokenResponse>(ClientNotification.ClientId, "ClientDeviceToken");
                Send(new FirebaseNotification(ClientDeviceToken.DeviceToken, ClientNotification.Notification));
            }
            return;
        }

        private async Task ProcessEmployeeNotifications()
        {
            var tmp = _employeeNotifications.ToList();
            _employeeNotifications.Clear();

            EmployeeDeviceTokensResponse EmployeeDeviceTokens = _rabbit.RpcRequest<string, EmployeeDeviceTokensResponse>("", "EmployeeDeviceToken");
            foreach (var EmployeeNotification in tmp)
            {
                foreach (var EmployeeDeviceId in EmployeeDeviceTokens.DeviceTokens)
                {
                    Send(new FirebaseNotification(EmployeeDeviceId, EmployeeNotification));
                }
            }
            return;
        }

        private async void Send(FirebaseNotification FirebaseNotification)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new FirebaseMessage(FirebaseNotification)), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("FIREBASE_SECRET"));  
  
            var response = await _client.PostAsync("https://fcm.googleapis.com/v1/projects/707924622826/messages:send", content);
        }
    }

    public class FirebaseMessage
    {
        public FirebaseNotification message { get; set; }

        public FirebaseMessage(){}
        public FirebaseMessage(FirebaseNotification message)
        {
            this.message = message;
        }
    }

    public class FirebaseNotification
    {
        public string token { get; set; }
        public Notification notification { get; set; }

        public FirebaseNotification(string DeviceToken, Notification Notification)
        {
            token = DeviceToken;
            notification = Notification;
        }

    }
    public class Notification
    {
        public string title { get; set; }
        public string body { get; set; }

        public Notification(string title, string body)
        {
            this.title = title;
            this.body = body;
        }
    }
}
