using Credit_Api.Models.innerModels;
using CreditService_Patterns.Models.innerModels;

namespace CreditService_Patterns.Models.responseModels;

public class ClientCreditsListForClientResponseDTO
{
    public required List<ClientCreditForClientResponse> CreditsList { get; set; }
    public required PaginationResponse Pagination { get; set; }
}
