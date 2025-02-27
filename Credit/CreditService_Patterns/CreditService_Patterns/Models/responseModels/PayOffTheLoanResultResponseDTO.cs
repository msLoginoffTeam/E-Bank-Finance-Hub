using CreditService_Patterns.Models.innerModels;

namespace CreditService_Patterns.Models.responseModels;

public class PayOffTheLoanResultResponseDTO
{
    public required Guid Id { get; set; }
    public required Guid CreditId { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required float Ammount { get; set; }
    public required float Excess {  get; set; }
    public required ClientCreditStatusEnum CreditStatus { get; set; }
}
