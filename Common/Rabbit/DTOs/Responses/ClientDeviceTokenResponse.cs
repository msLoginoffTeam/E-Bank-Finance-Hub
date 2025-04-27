namespace Common.Rabbit.DTOs.Responses
{
    public class ClientDeviceTokenResponse : RabbitResponse
    {
        public string DeviceToken { get; set; }

        public ClientDeviceTokenResponse() : base() {}
        public ClientDeviceTokenResponse(string DeviceToken) : base()
        {
            this.DeviceToken = DeviceToken;
        }
    }
}
