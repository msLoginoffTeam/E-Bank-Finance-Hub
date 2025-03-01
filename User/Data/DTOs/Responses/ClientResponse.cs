using UserApi.Data.Models;

namespace User_Api.Data.DTOs.Responses
{
    public class ClientResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }
        
        public bool IsBlocked { get; set; }

        public ClientResponse(Client Client)
        {
            Id = Client.Id;
            Email = Client.Email;
            FullName = Client.FullName;
            IsBlocked = Client.IsBlocked;
        }
    }
}
