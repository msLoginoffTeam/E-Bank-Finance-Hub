using CreditService_Patterns.Models.innerModels;

namespace CreditService_Patterns.Models.responseModels;

public class ClientCreditsListForClientResponseDTO
{
    public required List<ClientCreditForClientResponse> CreditsList { get; set; }
}
