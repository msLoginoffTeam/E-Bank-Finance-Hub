using Core.Data.Models;

namespace Core_Api.Data.DTOs.Requests
{
    public class CreateAccountRequest
    {
        public string Name { get; set; }
        public Currency Currency { get; set; }
    }
}
