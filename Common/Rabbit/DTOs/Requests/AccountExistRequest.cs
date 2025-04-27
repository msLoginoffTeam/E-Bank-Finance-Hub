namespace Common.Rabbit.DTOs.Requests
{
    public class AccountExistRequest
    {
        public Guid AccountId { get; set; }
        public Guid ClientId { get; set; }
        public string TraceId { get; set; }


		public AccountExistRequest() { }

        public AccountExistRequest(Guid AccountId, Guid ClientId, string TraceId)
        {
            this.AccountId = AccountId;
            this.ClientId = ClientId;
            this.TraceId = TraceId;
        }
    }
}
