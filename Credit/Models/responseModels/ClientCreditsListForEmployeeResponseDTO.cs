using CreditService_Patterns.Models.innerModels;

namespace CreditService_Patterns.Models.responseModels;

public class ClientCreditsListForEmployeeResponseDTO
{
    public required List<ClientCreditForEmployeeResponse> CreditsList { get; set; }
}
